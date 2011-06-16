#!/bin/sh
#
# I use this script to watch a french tv show with vlc .
#
# Ce script extrait l'URL MMS pour l'e'mission courante d' "On n'est pas couche'"
# et lance VLC.
#
# Copyright (C) 2011 Johan Burati <johan.burati@gmail.com>
# Code is licensed under GNU GPL license.
#

url=http://on-n-est-pas-couche.france2.fr/
# Get the current emission
current=$(curl -s $url | perl -lane 'if (/href="(\S.*\.asx)"/) { print $1}')
# Extract the video feed
video=$(curl -s $url$current |  perl -lane 'if (/href="(mms:\S.*\.wmv)"/) { print $1}')
# Startup VLC
vlc ${video} 2>&1 >/dev/null &
