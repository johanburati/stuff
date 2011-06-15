#!/usr/bin/perl
use strict;
use warnings;
use Getopt::Long;
use Proc::ProcessTable;
use Term::ANSIColor qw(:constants);

    my %base;
    my %desc;
    my %curr;
    my $max_ps = 5;


sub dumpps {
   use Data::Dumper;

	my ($pps) = @_;
	$Data::Dumper::Sortkeys = 1;
        $Data::Dumper::Useqq = 1;
	print Data::Dumper->Dump( [ $pps ], [ qw/*base/ ] );
}

sub getbase {
    my ($pbase, $pdesc) = @_;

    require "/etc/pss/base";
    our %base;
    foreach (keys %base) {
        $$pbase{$_} = $base{$_};
    }

    require "/etc/pss/desc";
    our %desc;
    foreach (keys %desc) {
        $$pdesc{$_} = $desc{$_};
    }
}

# get the list of running processes in a hash array
sub getps {
   my ($pps) = @_;

   my $ptable = new Proc::ProcessTable;
   foreach my $p (@{$ptable->table}){
      next if ($p->pid(  ) == $$);
      #print $p->fname,"\n";
      my ($psname) = split('/', $p->fname);

      if (not defined $$pps{$psname}) {
         $$pps{$psname} = 1;
      } else {
         $$pps{$psname}++;
      }
   }
}

# compare two hash array with processes
sub compareps {
   my ($pbase, $pcurr, $o_verbose) = @_;

   foreach (sort keys %{$pcurr}) {
      my $description = "";
      if ($o_verbose && defined $desc{$_}) { $description = " - ".$desc{$_}; }

      if (not defined $$pbase{$_}) {
         print GREEN," + ",$_,$description,RESET,"\n";
      } else {
         if ($$pcurr{$_} > $$pbase{$_} + $max_ps) {
            my $msg = sprintf(" > %s%s (%d>%d)\n", ,$_,$description,$$pcurr{$_},$$pbase{$_});
            print MAGENTA, $msg, RESET;
         } else {
            if ($description ne "") {
               print BOLD BLUE," ",$_,$description,RESET,"\n";
            }
         }
      }
   }
   foreach (sort keys %{$pbase}) {
      my $description = "";
      if ($o_verbose && defined $desc{$_}) { $description = " - ".$desc{$_}; }

      if (not defined $$pcurr{$_}) {
         print RED," - ",$_,$description,RESET,"\n";
      }
   }

}

sub overps {
   my ($pps) = @_;
   foreach (sort keys %{$pps}) {
      if ($$pps{$_} > $max_ps) {
         print CYAN," warn: there are ",$$pps{$_}," ",$_," processes running",RESET,"\n";
      }
   }
}

sub main {

   my ($o_dump, $o_verbose);
   GetOptions("d" => \$o_dump, "v" => \$o_verbose);

   getps(\%curr);

   if ($o_dump) {
      dumpps(\%curr);
   } else {
      getbase(\%base, \%desc);
      my $countb = scalar keys %base;
      my $countc = scalar keys %curr;
      print YELLOW; printf " [ processes count: %d (%-+d) ]", $countc, $countc - $countb; print RESET,"\n";
      compareps(\%base, \%curr, $o_verbose);
   }

}

main();

__END__

sub load_base {
   my $file = "pss";
   open(my $fh,"<",$file) or die "cannot open '$file' !";
   while(<$fh>) {
      chomp();
      $base{$_} = 1;

   }
   close $fh;
}

sub load_current {
   my $cmd="/bin/ps -eo comm | /usr/bin/uniq | /bin/sort";
   
   open(my $fh,"$cmd|") or die "cannot run '$cmd' !";
   while(<$fh>) {
      chomp();
      if (not defined $current{$_}) {
            $current{$_} = 1;
      } else {
            $current{$_}++;
      }
   }
   close $fh;
}

