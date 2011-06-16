#!/usr/bin/perl
#
#  A function to convert number of bytes in human readable format
#
# Copyright (C) 2011 Johan Burati <johan.burati@gmail.com>
# Code is licensed under GNU GPL license.
#
use strict;
use warnings;

sub human_readable  {
   my ($size) = @_;

# TiB
   if ($size >= 1099511627776)  {
      return sprintf("%.2f TiB", $size / 1099511627776);
# GiB
   }
   elsif ($size >= 1073741824) {
      return sprintf("%.2f GiB", $size / 1073741824);
# MiB
   }
   elsif ($size >= 1048576) {
      return sprintf("%.2f MiB", $size / 1048576);
# KiB
   }
   elsif ($size >= 1024) {
      return sprintf("%.2f KiB", $size / 1024);
# bytes
   }
   else {
      return sprintf("%.0f bytes", $size);
   }
}

sub main {
   printf "  TiB: %s\n",human_readable(1024*1024*1024*1024);
   printf "  GiB: %s\n",human_readable(1024*1024*1024);
   printf "  MiB: %s\n",human_readable(1024*1024);
   printf "  KiB: %s\n",human_readable(1024);
   printf "bytes: %s\n",human_readable(1023);
}

main();
__END__
