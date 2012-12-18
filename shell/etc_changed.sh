#!/bin/bash
#
# etc_changed - Archive changed file in /etc
#
# Copyright (C) 2011 Johan Burati <johan.burati@gmail.com>
# Code is licensed under GNU GPL license.
#
rpm_cmd="/bin/rpm --verify --all --nodeps --nolinkto --nodigest --noscripts --nosignature --nouser --nogroup --nomtime --nomode --nordev"

main () {
   files_list=$(/bin/mktemp /tmp/etc_changed.XXXX)
   $rpm_cmd 2>/dev/null > $files_list
   wc -l $files_list
   rm $files_list
}

main
exit 0
