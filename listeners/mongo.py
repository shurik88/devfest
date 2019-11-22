import pymongo
from pymongo import MongoClient
from bson.json_util import dumps
import pika
import json

class OpLogMessage(object):
    def __init__(self, collection:str, action:str, key:str):
        self.source = "tests"
        self.entity = collection
        self.action = "update" if action == "replace" else action
        self.data = { "source": "tests", "entity": self.entity, "operation": self.action, "key": key  }

def sendMessageToRabbitMq(msg: OpLogMessage):
    connection = pika.BlockingConnection(pika.ConnectionParameters('localhost'))
    channel = connection.channel()
    to = msg.entity + "." + msg.action
    channel.basic_publish(exchange='devfesttests',
                      routing_key=to,
                      body=json.dumps(msg.data))
    print("message to {0} sended".format(to))

client = MongoClient('mongodb://localhost:27018/devfest?replicaSet=replocal')

db = client.devfest

pipeline = [{
     "$match": { "operationType": { "$in":["insert", "delete", "update", "replace"] }}
     } ]

with db.tests.watch(pipeline) as stream:
    for change in stream:
        print(change)
        msg = OpLogMessage(change['ns']['coll'], change['operationType'], change['documentKey']['_id'])
        sendMessageToRabbitMq(msg)

print ("close")