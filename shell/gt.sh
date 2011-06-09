#!/bin/sh
# 
# This script use Google Translate to translate a word or phrase.
#
# Johan Burati (johan.burati@gmail.com)
# Code is licensed under GNU GPL license.
#
shopt -s nocasematch
eucjp=1
url="http://ajax.googleapis.com/ajax/services/language/translate"

eucjp_to_utf8 () {
   echo "$1" | iconv -sc -f euc-jp -t utf-8;
}

utf8_to_eucjp () {
   echo "$1" | iconv -sc -t euc-jp -f utf-8;
}

if [ $# -lt 1 ]
then
  echo "Usage: `basename $0` 'The World is yours' [en] [fr]"
  exit -1 
else
   input=$1;from=${2:-en}; to=${3:-fr};
   if [[ $eucjp && $from == "ja" ]]; then input=$(eucjp_to_utf8 "$input"); fi
   output=$(curl -s -d "v=1.0" --data-urlencode "q=${input}" -d "langpair=${from}|${to}" $url | perl -lane 'print $1 if /"translatedText":"(.+?)"/') 
   if [[ $eucjp && $to == "ja" ]]; then output=$(utf8_to_eucjp "$output"); fi 
   echo "$output"
fi
