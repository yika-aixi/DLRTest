#!/usr/bin/python
#import sys,UnityEngine
#from Icarus.DLR.Test import tt
import sys,clr

clr.AddReference("UnityEngine.CoreModule")
clr.AddReference("Assembly-CSharp")
from UnityEngine import *
from Icarus.DLR.Test import tt
test="Hello Wrold!"

def SetColor(image,color):
	image.color = color
	tt.Test(image)
	newobj = GameObject("newc")
	newobj = Resources.Load[TextAsset]("Script");
	return newobj.text
	