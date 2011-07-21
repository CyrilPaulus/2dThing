#!/bin/bash

#Needs mono 2.10 and SFML2.0, CSFML2.0
MONO=/opt/mono-2.10/bin/mono
SFML=./lib/

export LD_LIBRARY_PATH=$LD_LIBRARY_PATH:$SFML

$MONO 2dThing.exe -client