#!/bin/bash
#
# system_check - check logs for break-in
#
# Copyright (C) 2011 Johan Burati <johan.burati@gmail.com>
# Code is licensed under GNU GPL license.
#
highlight="\033[38;5;148m"
reset="\033[39m"

echo -e $highlight"- Messages from the kernel"$reset
#dmesg | egrep -v "reserve failed for wait|Disabled Privacy Extensions" | less
dmesg | egrep -i "\b(error|fail|warn|bug)" | less||

echo -e $highlight"- Last failed remote login"$reset
sudo lastb | perl -lane 'print if ($F[2] =~ /^\d/)';
echo -e $highlight"- Last succesful remote login"$reset
sudo lastb | perl -lane 'print if ($F[2] =~ /^\d/)' | head
