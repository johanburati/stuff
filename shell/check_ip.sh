#!/bin/bash
#
# check_ip - check the registered IP with the current IP
#
# Copyright (C) 2011 Johan Burati <johan.burati@gmail.com>
# Code is licensed under GNU GPL license.
#

registered_ip=$(dig +short bxl.burati.com @a.dns.gandi.net)
current_ip=$(curl -s http://keron.dip.jp/getip.php)

if [ "$registered_ip" == "$current_ip" ]; then
   echo "OK!"
else
   echo "Registered IP: $registered_ip"
   echo "Current IP: $current_ip"
fi
