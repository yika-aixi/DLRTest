#!/usr/bin/python
import UnityEngine
test="Hello Wrold!"
	
def SetColor(image,color):
	image.color = color
	newobj = UnityEngine.Resources.Load("Script");
	return newobj.text