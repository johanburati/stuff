#!/bin/sh

# Simple httpd honey
#
# Copyright (C) 2011 Johan Burati <johan.burati@gmail.com>
# Code is licensed under GNU GPL license.
#
netcat=$(which nc 2>/dev/null)
honey_port=80
honey_log=$HOME/tmp/honey.log
honey_lock=$HOME/tmp/honey.lock
honey_banner="MS-IIS WEB SERVER 5.0\r"

loop() {
    while [ -f ${honey_lock} ]; do
        $netcat -lp $honey_port -c 'echo HTTP/1.0 200 OK' 2>&1 >> ${honey_log}
        echo "`date +'%Y-%m-%d %H:%M'` Attempted connection on honey_port $honey_port" >> ${honey_log}
        echo "-------------------------------------------" >> ${honey_log}
    done
}

quit() {
  echo "<Ctrl-C> quitting ..."
  rm ${honey_lock}
  exit 0
}

trap 'quit' 2
if [ ! -x "$netcat" ]; then
   echo "Oops, looks like netcat is not installed on your system !"
   exit -1
fi
echo "Starting honey on honey_port $honey_port, hit <Ctrl-C> to quit..."
touch ${honey_lock}
loop
