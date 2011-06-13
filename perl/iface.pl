#!/usr/bin/perl
#
# This script show the amount of data receiveid and sent on each interface
#
# Johan Burati (johan.burati@gmail.com)
# Code is licensed under GNU GPL license.
#
use strict;
use warnings;
use Getopt::Long;
use Term::ANSIColor qw(:constants);

sub gib {
   my ($size) = @_;
  return sprintf("%.3f GiB", $size / 1073741824);
}

sub mib {
   my ($size) = @_;
  return sprintf("%.3f MiB", $size / 1048576);
}


sub get_slaves {
   my ($bond) = @_;
   my $active = "";
   my $pasive = "";
   open (my $fh, '<', "/proc/net/bonding/$bond") or die "Stopped";
   while (<$fh>) {
      $active=$1 if (/Currently Active Slave: (\S+)/);
      $pasive=$1 if ((/Slave Interface: (\S+)/) && ($1 ne $active));
   }
   close($fh);
   return sprintf(" (%s*,%s)",$active,$pasive);
}

sub main {
   # deal with the command line options
   my ($o_header, $o_mib, $o_bytes, $o_color);
   GetOptions("h" => \$o_header, "m" => \$o_mib, "b" => \$o_bytes, "c" => \$o_color);

   # regex to grab the value from /proc/net/dev
   my $regex = '^\s*(\S+):\s*' .
               '(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+' .
               '(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)';

   printf "%-6s  %-12s %-12s\n", "Iface", "Received", "Transmitted" if $o_header;
   open (my $fh, '<', "/proc/net/dev") or die "Stopped";
   while(<$fh>) {
      my ($iface,
	  $rx_bytes, $rx_packets, $rx_errs, $rx_drop, $rx_fifo, $rx_frame, $rx_compressed, $rx_multicast,
          $tx_bytes, $tx_packets, $tx_errs, $tx_drop, $tx_fifo, $tx_frame, $tx_compressed, $tx_multicast
	 ) =  /$regex/;

      if (defined $iface) {
	 my $slaves="";
         if ($iface =~ /^bond/)  { $slaves = get_slaves($iface);}
	 if ($o_color) { if ($rx_errs + $rx_drop + $tx_errs + $tx_drop > 0) { print RED } else { print GREEN }}
	 if ($o_bytes) {
            printf "%6s: %12s %12s %12s\n",$iface,$rx_bytes,$tx_bytes,$slaves;
         } elsif ($o_mib) {
            printf "%6s: %12s %12s %12s\n",$iface,mib($rx_bytes),mib($tx_bytes),$slaves;
	 } else {
	    # print the values in GiB by default
            printf "%6s: %12s %12s %12s\n",$iface,gib($rx_bytes),gib($tx_bytes),$slaves;
	 }
	 if ($o_color) { print RESET }
      }
   }
   close($fh);
}

main();
__END__
