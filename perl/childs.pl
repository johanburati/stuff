#!/usr/bin/perl

use strict;
use warnings;

my $pid = fork;

die "Cannot fork: $!" unless defined $pid;

if ($pid == 0) {
    print "Child start\n";
    my $end;
    local $SIG{HUP} = sub { $end = 1 };

    until ($end) {
        print "Sleep 1\n";
        sleep 1;
    }
    function2();
}
else {
    print "Parent start\n";
    sleep 5;
    kill HUP => $pid;
    waitpid($pid, 0);
}

sub function2 {
    print "END\n";
}
