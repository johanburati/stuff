#!/bin/awk -f
#
# display memory usage per user
#
# Johan Burati (johan.burati@gmail.com)
# Code is licensed under GNU GPL license.

BEGIN {
    ps = "ps -eo user,rss | sed 1d"
    while ((ps | getline) > 0) {
        memtotal[$1] += $2
    }
    close(ps)

    for (name in memtotal)
        printf("%-8s %10dKB\n", name, memtotal[name]) | "sort -k2 -rn"
}
