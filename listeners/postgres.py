import psycopg2
import sys
import pika
from psycopg2.extras import LogicalReplicationConnection
import json
import re

class WalMessage(object):
    def __init__(self, entity:str, action:str, key:str):
        self.entity = entity
        self.action = action.lower()
        self.source = 'disciplines'
        self.data = { "source": self.source, "entity": self.entity, "operation": self.action, "key": key  }
        

def parseWalMessage(text: str) -> WalMessage:
    match = re.search('table\s(?P<tablename>[^:]+):\s(?P<operation>\w+):.+\"Id\"[^:]+:(?P<idvalue>[^ ]+)', text)
    tableName = match.group('tablename')
    action = match.group('operation')
    key = match.group('idvalue')
    return WalMessage('disciplines' if tableName == 'public."Disciplines"' else tableName, action,key)

def sendMessageToRabbitMq(msg: WalMessage):
    connection = pika.BlockingConnection(pika.ConnectionParameters('localhost'))
    channel = connection.channel()
    to = msg.entity + "." + msg.action
    channel.basic_publish(exchange='devfestdisciplines',
                      routing_key=to,
                      body=json.dumps(msg.data))
    print("message to {0} sended".format(to))



conn = psycopg2.connect(dbname='disciplines', user='myuser', 
                        password='mypass', host='localhost', 
                        connection_factory =LogicalReplicationConnection)

        

cur = conn.cursor()
try:
    cur.drop_replication_slot('pytest')
except:
    print("pytest already deleted")


cur.create_replication_slot('pytest', output_plugin ='test_decoding')
# CREATE_REPLICATION_SLOT “pytest” LOGICAL “test_decoding” wal2json

cur.start_replication(slot_name ='pytest', decode=True)

def consume(msg):
    if(msg.payload.find('table') != -1):
        print(msg.payload)
        message = parseWalMessage(msg.payload)
        sendMessageToRabbitMq(message)
    


cur.consume_stream(consume)
cur.drop_replication_slot( 'pytest')


