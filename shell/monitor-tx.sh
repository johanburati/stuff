#!/bin/bash
#
# monitor_tx - monitor upstream network traffic.
#
# Description:
# My ISP only allow 30 GiB upstream transfer by day.
# If that limit is exceeded they might terminate the contract.
# This script should be run from the crontab to keep
# an eye on the daily upstream transfert and take
# action before the daily limit is reached.
#
# Requirements:
# vnstat
#
# Copyright (C) 2011 Johan Burati <johan.burati@gmail.com>
# Code is licensed under GNU GPL license.


tib=$((10 * 1024 * 1024))
gib=$((10 * 1024 ))
mib=$((10 ))

# take some action when it reachs 20 GiB
max_tx=$(( 20 * $gib * 100))

tx_to_kb_xxx () {
    tx=${1%.*};   # we drop the number after the dec
    unit=${1#* }

    case ${unit} in
    "TiB") tx=$(($tx * $tib * 100 ))
        ;;
    "GiB") tx=$(($tx * $gib * 100 ))
        ;;
    "MiB") tx=$(($tx * $mib * 100 ))
        ;;
    esac
    echo $tx
}

tx_to_kb () {
    # extract the transfered amount and unit from the value returned by vnstat
    # a typical value returned by vnstat looks like this '1.74 GiB'
    tx=${1% *};     # we take the first part before the space
    tx=${tx//./}    # we take out the dot, at that point tx='174'
    unit=${1#* }    # for the unit we take the last part after the space, unit='GiB'

    # we mulitply the tx depending on the unit, so we have the transfered amount in KB
    case ${unit} in
    "TiB") tx=$(($tx * $tib ))
        ;;
    "GiB") tx=$(($tx * $gib ))
        ;;
    "MiB") tx=$(($tx * $mib ))
        ;;
    esac
    echo $tx        # we return the transfered amount in KB, in the example it's '1781760'
}

# this function check if the command passed in argument exist, if it does not it exits the script
check_if_command_exists () {
    command -v $1 >/dev/null
    if [ "$?" -ne 0 ]; then echo  "Error: $1 command not found !"; exit -1; fi
}

# return current tx in KB
get_current_tx () {
    check_if_command_exists vnstat
    current_tx=$(vnstat --oneline --iface eth0 | awk -F\; '{ print $5 }')
    echo $(tx_to_kb "$current_tx")
}

take_action () {
    logger -s "monitor_tx: $1"
    # echo $1 | mailx -s "monitor_tx" johan.burati@gmail.com
    # kill network hogs
    pkill "amuled|transmission|torrent"
}

main () {
    curr_tx=$(get_current_tx)
    if [ $((curr_tx)) -gt $((max_tx)) ]; then
            take_action "current traffic (${curr_tx} KiB) reached the limit (${max_tx} KiB) !"
    fi
}

main
exit 0
