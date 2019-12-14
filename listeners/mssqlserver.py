import argparse
import pyodbc
from changetracking import EntityAction, EntityChangeMsg
from rabbitmq import sendMessageToRabbitMq
import time





parser = argparse.ArgumentParser(description='Ms Sql Server change tracking notifier using rabbitmq.')
parser.add_argument('-s','--sqlcon',  type=str, help='sql server connection string', required=True)
parser.add_argument('-r', '--rabbitcon', type=str, help='rabbit mq connection string', required=True)
parser.add_argument('-p','--periodicity', type=int, default=10, help='periodicity(in minutes) in which listener get changes')
parser.add_argument('-e','--exchange', type=str, help='exchange name to push', required=True)
parser.add_argument('-t','--tables', type=str, help='tables to listen', nargs='+', required=True)
parser.add_argument('-n','--name', type=str, help='name of listener(source)', required=True)

args, unknown = parser.parse_known_args()


class SqlEntityChange(object):
    def __init__(self, version:int, table:str, operation:str, key:object):
        self.version = version
        self.table = table
        self.operation = operation
        self.key = key


#'Driver={SQL Server};Server=localhost;Database=db_name;Uid=myuser;Pwd=mypass;'

operationConverter = {
    'U': EntityAction.update,
    'I': EntityAction.insert,
    'D': EntityAction.delete
}

def getChanges(sqlCon: str, table:str, minVersion: int) -> [SqlEntityChange]:
    conn = pyodbc.connect(sqlCon)
    
    cursor = conn.cursor()
    cursor.execute("""select (case when exists(select top 1 * from sys.change_tracking_tables 
                        where OBJECT_SCHEMA_NAME(object_id) + '.' + Object_Name(object_id) = ?)
                        then 1 else 0 end) tableExists""", table)
    row = cursor.fetchone()
    if(row.tableExists == 0):
        return []
    
    changes =  []
    cursor.execute('select * from changetable(changes ' + table + ', ?) as entityChanges', minVersion)
    idColumns = [column[0] for column in cursor.description if not column[0].startswith('SYS_CHANGE')]
    for change in cursor:
        changes.append(SqlEntityChange(change.SYS_CHANGE_VERSION, 
                        table,
                        change.SYS_CHANGE_OPERATION, 
                        getattr(change, idColumns[0]) if len(idColumns) == 1 
                            else  { id : getattr(change, id) for id in idColumns } ))
        print (change)
    return changes
    

print(args)

#[filename for path in dirs for filename in os.listdir(path)]
while True:
    with open('mssqlserver_last_sync_version.txt', 'r') as reader:
        version = int(reader.readline())
    entityChanges = [change for tableName in args.tables for change in getChanges(sqlCon = args.sqlcon, table = tableName, minVersion = version)]
    entityChanges.sort(key = lambda x: x.version)
    for change in entityChanges:
        sendMessageToRabbitMq(
                    msg = EntityChangeMsg(source = args.name,
                        entity = change.table.split('.')[-1],
                        action = operationConverter[change.operation],
                        key = change.key), 
                    rabbitmqCon = args.rabbitcon,
                    exchangeName = args.exchange)
        with open('mssqlserver_last_sync_version.txt', 'w') as writer:
            writer.write(str(change.version))

    time.sleep(args.periodicity * 60)


sqlChanges = getChanges(args.sqlcon, args.tables[0], 10)
for sqlChange in sqlChanges:
    print(sqlChange)
