#!/usr/bin/perl
#
# Verify the BodyLength and the CheckSum field of a FIX message
#
# Johan Burati (johan.burati@gmail.com)
# Code is licensed under GNU GPL license.
#

while(<DATA>) {
   print "> ",$_;
   my $BodyLength = 0;
   my $CheckSum = 0;
   my @fields = split /\|/,$msg;

   foreach my $field (@fields) { 
      if ( $field =~ /^10=/) { next; }   # skip CheckSum field
      foreach my $char (split(//,$field)) { $CheckSum += ord($char); } $CheckSum += 1;
      if ( $field =~ /^9=/) { next; }    # skip BodyLength field
      $BodyLength += length($field);
   }
   $CheckSum %= 256;

   printf "9=%d|10=%03d\n",$BodyLength, $CheckSum;
}
exit(0);

__DATA__
8=FIX.4.2|9=70|35=4|49=A|56=XYZ|34=129|52=20100302-19:38:21|43=Y|57=LOL|123=Y|36=175|10=190|
8=FIX.4.2|9=71|35=4|49=A|56=XYZ|34=129|52=20100302-19:38:21|43=Y|57=LOL|123=Y|36=175|10=191|
