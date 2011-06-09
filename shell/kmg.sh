#!/bin/bash
#
# convert bytes to KB,MB or GB
#
# Johan Burati (johan.burati@gmail.com)
# Code is licensed under GNU GPL license.

kmg () {
   value=$1
   kilo=$( echo "scale=2; $value / 1024" | bc )
   kiloint=$( echo "$value / 1024" | bc )

   mega=$( echo "scale=2; $kilo / 1024" | bc )
   megaint=$( echo "$kilo / 1024" | bc )

   giga=$( echo "scale=2; $mega / 1024" | bc )
   gigaint=$( echo "$mega / 1024" | bc )

   if [ $kiloint -lt 1 ] ; then
   echo "$value bytes"
   elif [ $megaint -lt 1 ] ; then
   echo "${kilo}KB"
   elif [ $gigaint -lt 1 ] ; then
   echo "${mega}MB"
   else
   echo "${giga}GB"
   fi
}

if [ $1 ]; then
    kmg $1
fi
exit 0
