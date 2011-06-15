#!/bin/sh
url=http://on-n-est-pas-couche.france2.fr/IMG/asx/onpc-101.asx
video=$(curl -s $url |  perl -lane 'if (/href="(.*\.asx)"/) { print $1}')
vlc ${url}${video} 2>&1 >/dev/null &
