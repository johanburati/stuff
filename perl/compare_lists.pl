#!/usr/bin/perl
#
# This script compare two lists and print the differences
# run 'perldoc compare_lists.pl' for more information.
#
# Copyright (C) 2011 Johan Burati <johan.burati@gmail.com>
# Code is licensed under GNU GPL license.
#
use strict;
use warnings;
use Getopt::Long;
use Term::ANSIColor qw(:constants);

my %list1;
my %list2;

# load a list from file
sub load_list {
   my ($plist, $file) = @_;
   open(my $fh,"<",$file) or die "Stopped '$!'";
   while(<$fh>) {
      chomp();
      my @f = split(/\s+/);
      $$plist{$_} = 1;
   }
   close $fh;
}

# compare two lists 
sub compare_lists {
   my ($plist1, $plist2) = @_;

   foreach (sort keys %{$plist2}) {
      if (not defined $$plist1{$_}) {
         print GREEN," + ",$_,RESET,"\n";
      }
   }
   foreach (sort keys %{$plist1}) {
      if (not defined $$plist2{$_}) {
         print RED," - ",$_,RESET,"\n";
      }
   }
}

sub main {
   if ($#ARGV + 1 < 2) {
      print STDERR "Usage compare_lists.pl file1 file2\n"; 
      exit(-1);
   }
   my $file1 = $ARGV[0];
   my $file2 = $ARGV[1];

   load_list(\%list1, $file1);
   load_list(\%list2, $file2);
   print YELLOW; printf "* %s: %d | %s: %d", $file1, scalar keys %list1, $file2, scalar keys %list2; print RESET,"\n";
   compare_lists(\%list1, \%list2);
}

main();

__END__

=headd1 NAME

   compare_lists - compare two lists, such as list of installed rpm, modified file

=head1 SYNOPSIS

   compare_lists file1 file2

=head1 DESCRIPTION

   Read two lists from files, compare them and print the differences.
   Usefull to compare such thing as a list of rpm from two hosts.

=head1 OPTIONS

   file1 first list
   file2 second list

=head1 AUTHOR

   Johan Burati <johan.burati@gmail.com>

=cut

