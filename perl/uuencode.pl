#!/usr/bin/perl
#
# Uuencode a file
# 
# Johan Burati (johan.burati@gmail.com)
# Code is licensed under GNU GPL license.
#
die "Usage: $0 FileToEncode\n" if (!defined ($ARGV[0]));
open (INPUT, $ARGV[0]) or die "Can't open $ARGV[0]: $!\n";
binmode INPUT;
print "begin 644 $ARGV[0]\n";
print pack ("u", $bloc) while read (INPUT, $bloc, 45);
print "`\n";
print "end\n";
close (INPUT);
exit 0;
