from enum import Enum

class EntityAction(Enum):
    insert = 1
    update = 2
    delete = 3

class EntityChangeMsg(object):
    def __init__(self, source:str, entity:str, action:EntityAction, key:object):
        self.source = source
        self.entity = entity
        self.action = action.name
        self.data = { "source": self.source, "entity": self.entity, "operation": self.action, "key": key  }