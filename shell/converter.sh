#!/bin/bash
#
# converter - convert PDF to JPG
#
# Copyright (C) 2011 Johan Burati <johan.burati@gmail.com>
# Code is licensed under GNU GPL license.
#

main () {
   filename=$1
   shortname=${filename%.*}
   extension=${filename##*.}

   if [ "$extension" == "pdf" ]; then
      if [ -f $shortname.jpg ]; then
         echo Error: file $shortname.jpg already exists !
      else
         echo convert -density 400 $filename -scale 3000x2000 $shortname.jpg
      fi
   else
      echo Error: input file must be a pdf file !
   fi
}

main
exit 0
