#!/bin/sh
#
# This script use Google Translate to translate a word or phrase.
#
# In Japan, the EUC-JP encoding is heavily used on Unix-like operating systems,
# Google Translate on the other hand deals with UTF-8.
# Therefore in case the user use a EUC-JP compatible terminal, we need to convert
# the text between EUC-JP and UTF-8.
#
# I do not know of any way to determine the encoding the user is using,
# so for this script to work on a EUC-JP terminal the user should export
# an ENCODING=EUC-JP variable.
#
# Johan Burati (johan.burati@gmail.com)
# Code is licensed under GNU GPL license.
#
shopt -s nocasematch
url="http://ajax.googleapis.com/ajax/services/language/translate"

# In Japan, the EUC-JP encoding is heavily used by Unix or Unix-like operating systemsdd
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
   if [[ $from == "ja" && $ENCODING == "EUC-JP" ]]; then input=$(eucjp_to_utf8 "$input"); fi
   output=$(curl -s -d "v=1.0" --data-urlencode "q=${input}" -d "langpair=${from}|${to}" $url | perl -lane 'print $1 if /"translatedText":"(.+?)"/') 
   if [[ $to == "ja" && $ENCODING == "EUC-JP" ]]; then output=$(utf8_to_eucjp "$output"); fi 
   echo "$output"
fi
