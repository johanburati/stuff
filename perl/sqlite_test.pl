#!/usr/bin/perl
#
#  Just a test to see how to deal with SQLite db from perl
#
# Johan Burati (johan.burati@gmail.com)
# Code is licensed under GNU GPL license.
#
use strict;
use warnings;
use DBI;
$|=1;


sub main {
  
  my $menu = <<END;
1. [l]ist all records
2. [a]dd a record
3. [d]elete a record
0. [q]uit

END
  
  print $menu;
  
  while(1) {
    print "[l]ist,[a]dd,[d]elete,[q]uit - choice? ";
    chomp( my $c = <STDIN>); $c=lc($c);
    
    print "----------------\n";
    if ( lc($c) =~ /0|q/) {
      last;
     } 
     elsif (lc($c) =~ /1|l/) {
        list_records();        
     }
     elsif (lc($c) =~ /2|a/) {
        menu_add_record();
     }
     elsif (lc($c) =~ /3|d/) {
        menu_del_record();
      }
     else {
        print "wrong choice !\n";
     }
     print "----------------\n";
  }
}

# db_check_record - check if a record exist in the db
# 1 -> record exist
# 0 -> record does not exist
sub db_check_record {
  my ($firstname, $lastname) = @_;

  my $db = DBI->connect("dbi:SQLite:bbc.db", "", "", {RaiseError => 1, AutoCommit => 0});
  my $query_string = "SELECT * FROM ppl WHERE firstname='$firstname' AND lastname='$lastname'";
  my $query = $db->prepare($query_string);
  $query->execute() or print STDERR "Error(execute query):" . $query->errstr;
  $query->fetchrow_array();
  my $rows = $query->rows();
  $db->disconnect();
  return $rows;
}

sub read_name {    
  print "firstname: ";
  my $firstname = <STDIN>;
  chomp($firstname);
  $firstname = ucfirst($firstname);
  print "lastname: ";
  my $lastname = <STDIN>;
  chomp($lastname);
  $lastname = ucfirst($lastname);
  return($firstname, $lastname);
}

sub db_add_record {
   my ($firstname, $lastname) = @_;
   
   my $db = DBI->connect("dbi:SQLite:bbc.db", "", "", {RaiseError => 1, AutoCommit => 1});
   my $query_string = "INSERT INTO ppl(firstname,lastname) VALUES ('$firstname','$lastname')";  
   my $rc = $db->do($query_string);
  if ($rc == 1) {
     print "The record have been added.\n";
   } else { 
     print STDERR "Could not add record.\n";
   }
   $db->disconnect();
}

sub menu_add_record {
  my ($firstname, $lastname) = read_name();
  
  if (db_check_record($firstname, $lastname)) {
    print STDERR "This record already exist!\n";
  } else {
    db_add_record($firstname, $lastname);
  }
}

sub db_del_record {
  my ($id) = @_;
  
  my $db = DBI->connect("dbi:SQLite:bbc.db", "", "", {RaiseError => 1, AutoCommit => 1});
  my $query_string = "DELETE FROM ppl WHERE id='$id'";  
  my $rc = $db->do($query_string);
  if ($rc == 1) {
    print "The record $id has been deleted.\n";
  } else {
    print "Could not find record $id.\n";
  }
  $db->disconnect(); 
}

sub menu_del_record {
  print "id of record to delete: ";
  my $id = <STDIN>;
  chomp($id);

  db_del_record($id);
}

sub list_records {
   printf "  id | Name \n";
   my $db = DBI->connect("dbi:SQLite:bbc.db", "", "", {RaiseError => 1, AutoCommit => 0});
   my $query_string = "SELECT * FROM ppl";
   my $query = $db->prepare($query_string);
   $query->execute();
   my ($id, $firstname, $lastname);
   $query->bind_columns(\$id, \$firstname, \$lastname);
   while($query->fetch()) {
     printf " %3d | %s %s\n",$id,$firstname,$lastname;
   } 
   $db->disconnect();
}

main();
exit(0);
__END__
