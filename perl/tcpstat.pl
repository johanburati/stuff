#!/usr/bin/perl
#
# Wrapper for the Unix 'netstat' command.
# Run 'perldoc tcpstat.pl' for more information.
#
# Copyright (C) 2011 Johan Burati <johan.burati@gmail.com>
# Code is licensed under GNU GPL license.
#
use strict;
use warnings;

# Use the -n option so netstat does not resolve the hostname, quicker
chomp (my $netstat = `which netstat`); $netstat .= " -n";
my @states = ('ESTABLISHED','SYN_SENT','SYN_RECV','FIN_WAIT1','FIN_WAIT2',
               'TIME_WAIT','CLOSE','CLOSE_WAIT','LAST_ACK','LISTEN','CLOSING','UNKNOWN');

sub main {
  my ($delay,$count) = (1,0);
  my $lcount = 1;
  
  if ($#ARGV == 1) {
    $delay = $ARGV[0];
    $count = $ARGV[1];
  }
  
  do {
    my %counts;  
    if ($lcount == 1) { print_header(\@states); $lcount=25;} else { $lcount--; }
    
    get_tcpstat(\@states, \%counts);
    print_tcpstat(\@states, \%counts);
    sleep($delay) if (--$count);
  } while ($count);
}

# print_header - print the header for the tcp connections status
sub print_header {
  my ($pstates) = @_;

  my $header = "\n   "; foreach my $state (@$pstates) { $header .= sprintf "%11s", lc($state);} $header .= "\n";
  print $header;
}

# get_tcpstat - get the current tcp connections status using the netstat command
sub get_tcpstat {
  my ($pstates, $pcounts) = @_;

  open(my $cmd, "$netstat |") or die "Error: $!";
  while(<$cmd>) {
    foreach my $state (@$pstates) {
      if (/$state/) {
        $$pcounts{$state} = defined $$pcounts{$state}? $$pcounts{$state}+1: 1;
        next;
      }
    }
  }
  close $cmd;
}

# print_tcpstat - print the current tcp connections status on one line
sub print_tcpstat {
  my ($pstates, $pcounts) = @_;

  my $line = "";
  foreach my $state (@$pstates) {
    my $value = defined $$pcounts{$state}? $$pcounts{$state}: 0;
    $line .= sprintf("%11d",$value);
  }
  $line .= "\n";
  print $line;

}

main();
exit 0;
__END__

=head1 NAME

   tcpstat - report network connections state

=head1 SYNOPSIS

   tcpstat [ delay ] [ count ]

=head1 DESCRIPTION

   tcpstat reports network connections state every seconds
   or on a sampling period of 'delay' if specified. 
   tcostat keeps running or run for 'count' times if specified. 

=head1 OPTIONS

   delay    Sampling period interval in seconds.
   count    Number of times to report connections state.

=head1 AUTHOR

   Johan Burati <johan.burati@gmail.com>

=cut

