#!/bin/bash
#
# display memory usage per user
#
# Johan Burati (johan.burati@gmail.com)
# Code is licensed under GNU GPL license.

awk 'BEGIN {
    printf "--- mem usage per user (KB) ---\n"

    cmd = "ps -eo user,rss | sed 1d"
    while ((cmd | getline) > 0) {
        memtotal[$1] += $2
    }
    close(cmd)

    for (name in memtotal)
        printf("%-8s %10d\n", name, memtotal[name]) | "sort -k2 -rn"
}'
#ipcs -m | sed 1,2d | awk '{ SUM +=$5 } END { printf "%dMB\n", SUM/1024/1024 }'
awk 'BEGIN {
    printf "--- mem used by IPC (KB) ---\n"
    cmd = "ipcs -m | sed 1,2d"
    while ((cmd | getline) >0) {
        shmtotal += $5
    }
    close(cmd)
#    cmd = "ipcs -s | sed 1,2d"
#    while ((cmd | getline) >0) {
#        semtotal += $5
#    }
#    close(cmd)
    cmd = "ipcs -q | sed 1,2d"
    while ((cmd | getline) >0) {
        msqtotal += $5
    }
    close(cmd)
    printf("%-8s %10d\n", "shm", shmtotal/1024)
#    printf("%-8s %10d\n", "sem", semtotal/1024)
    printf("%-8s %10d\n", "msq", msqtotal/1024)
    system("")
}'
