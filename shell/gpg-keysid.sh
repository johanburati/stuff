#!/bin/bash
#
# gpg-keysid.sh - list private and public keys short id.
#
# Copyright (C) 2011 Johan Burati <johan.burati@gmail.com>
# Code is licensed under GNU GPL license.

cmd=gpg
if which $cmd &>/dev/null; then
    $cmd --list-secret-keys --with-colon | awk -F:  '/sec/ { keyid=substr($5,9,8); sub(/ <.*>/,"",$10); print "pvt:"$10":"keyid}'
    $cmd --list-keys --with-colon | awk -F:  '/pub/ { keyid=substr($5,9,8); sub(/ <.*>/,"",$10); print "pub:"$10":"keyid}'
else
    echo "Error: could not find $cmd !"
    exit 1
fi
exit 0
