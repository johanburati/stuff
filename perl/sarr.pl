#!/usr/bin/perl
#
# This script format the output of sar command,
# run 'perldoc sarr.pl' for more information.
#
# Copyright (C) 2011 Johan Burati <johan.burati@gmail.com>
# Code is licensed under GNU GPL license.
#
use strict;
use warnings;
use Getopt::Long;
use Term::ANSIColor qw(:constants);

my $threshold_yellow = 40;
my $threshold_red = 80;

# deals with the options
my $sar_cmd = "env LANG=C /usr/bin/sar -r";
my ($o_start, $o_end, $o_filename, $o_interval, $o_mega, $o_color);
GetOptions("s:s" => \$o_start, "e:s" => \$o_end, "f:s" => \$o_filename, "i:s" => \$o_interval, "m" => \$o_mega, "c" => \$o_color);
$sar_cmd .= " -s $o_start" if defined $o_start;
$sar_cmd .= " -e $o_end" if defined $o_end;
$sar_cmd .= " -f $o_filename" if defined $o_filename;
$sar_cmd .= " -i $o_interval" if defined $o_interval;

# find out number of rows that fit on the screen
my $stty = `/bin/stty -a`; my ($row) = $stty =~ /rows (\d+)/; my $crow = 0;
my $header = "\n           kbmemfree  kbmemused  %memused\n";
$header    = "\n           mbmemfree  mbmemused  %memused\n" if $o_mega;

# main loop
open (my $fh, '-|', $sar_cmd) or die "Stopped";
while(<$fh>) {
   print if ($.==1);
   print $header  if ($crow == 0);
   my ($time, $kbmemfree, $kbmemused, $memused, $kbbuffers, $kbcached, $kbswpfree, $kbswpused, $swpused, $kbswpcad) = split(/\s+/);
   if ( $kbmemfree =~ /^\d+$/ ) {
      my $kbmemtotal = $kbmemfree + $kbmemused;
      my $kbmemused2 = $kbmemused - $kbbuffers - $kbcached;
      my $kbmemfree2 = $kbmemfree + $kbbuffers + $kbcached;
      my $memused2 = $kbmemused2 / $kbmemtotal * 100;

      if ($o_mega) {
         $kbmemfree2 /= 1024;
         $kbmemused2 /= 1024;
      }

      if (! $o_color) {
         printf("%s %10d %9d %9.02f\n",$time,$kbmemfree2,$kbmemused2,$memused2);
      } else {
         if ($memused2 < $threshold_yellow) { print GREEN; } elsif ($memused2 < $threshold_red) { print YELLOW; } else { print RED; }
         printf("%s %10d %9d %9.02f\n",$time,$kbmemfree2,$kbmemused2,$memused2);
         print RESET;
      }
   }
   $crow++; $crow = 0 if ($crow == $row);
}
close($fh);
exit(0);
__END__

=head1 NAME

   sarr - report system memory activity information.

=head1 SYNOPSIS

   sarr [-s hh:mm:ss] [-e hh:mm:ss] [-f FILE] [-i INTERVAL] [-m] [-c]

=head1 DESCRIPTION

   Format the output of the 'sar' command to give a more relevant view of the system memory usage.
   Add (kbbuffers+kbcached) to kbmemfree and substract if from kbmemused and recalculate the %memused


=head1 OPTIONS

   -s	 Start time.
   -e	 End time.
   -f	 File holding the information (ex.: /var/log/sa/sa13)
   -m	 Print the value in megabytes.
   -c	 Use color, green and red if the usage is over the threshold.

=head1 AUTHOR

   Johan Burati <johan.burati@gmail.com>

=cut
