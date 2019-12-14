import pika
from changetracking import EntityChangeMsg
from json import dumps


def sendMessageToRabbitMq(msg: EntityChangeMsg, rabbitmqCon: str, exchangeName: str):
    connection = pika.BlockingConnection(pika.ConnectionParameters(rabbitmqCon))
    channel = connection.channel()
    to = msg.entity + "." + msg.action
    channel.basic_publish(exchange=exchangeName,
                      routing_key=to,
                      body=dumps(msg.data))
    channel.close()
    connection.close()