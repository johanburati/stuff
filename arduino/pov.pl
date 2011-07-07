#!/usr/bin/perl
#
# This script generate a pov sketch from a pattern file.
# Run 'perldoc pov.pl' for more information.
#
# Copyright (C) 2011 Johan Burati <johan.burati@gmail.com>
# Code is licensed under GNU GPL license.
#
use strict;
use warnings;

sub print_header {
   print <<HEADER;
#define LEDPIN 7
#define SWPIN 6
#
byte bitmap[] = {
HEADER
}

sub print_pattern {
   my ($pattern_file) = @_;

   open(my $fh, "$pattern_file") or die "Error: $!";
   while (<$fh>) {
      chomp();
      s/\./0/g; # replace "." with "0"
      s/\@/1/g; # replace "@" with "1"
      print "0b0$_,\n";
   }
   close($fh);
}

sub print_footer {
   print <<FOOTER;
0b10000000
};

unsigned char pos = 0;
unsigned char i = 0;

void setup() {
   for(char pin = LEDPIN; pin < LEDPIN + 7; pin++) {
     pinMode(pin, OUTPUT);
   }
   pinMode(SWPIN, INPUT);
   digitalWrite(SWPIN, HIGH);
}

void loop() {
   if(digitalRead(SWPIN) == LOW) {
     delay(6);
     pos = 0;
     while(bitmap[++pos] != 0b10000000) {
       for(i = 0; i < 7; i++) {
         digitalWrite(LEDPIN + 6 - i, (bitmap[pos] >> i) & 0b00000001);
       }
       delay(1);
     }
     for(i = LEDPIN; i < LEDPIN + 7; i++) {
       digitalWrite(i, LOW);
     }
   }
}
FOOTER
}

sub main {
   if ($#ARGV + 1 < 1) {
      print "Usage: pov graph.txt\n"; exit(-1);
   }
   my $pattern_file = $ARGV[0];
   print_header();
   print_pattern($pattern_file);
   print_footer();
}

main();
exit(0);
__END__
=head1 NAME

   pov - Generate a pov sketch for a given pattern file.

=head1 SYNOPSIS

   pov [pattern_file]

=head1 DESCRIPTION

   Generate a pov sketch for a given pattern file.
   The pattern file consist of row of seven pixel,
   "." means that the pixel is OFF, "@" means that the pixel is ON,
   you should see what the pattern is about by tidling your head to the right.

=head1 OPTIONS

   -h    Display the header
   -m    Display the value in MiB (default GiB)
   -b    Display the value in bytes (default GiB)
   -c    Use color, green and red if there are any kind of error on the interface

=head1 AUTHOR

   Johan Burati <johan.burati@gmail.com>

=cut

