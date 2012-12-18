my $pat_ip_address = qr/(\d{1,3} \.
        \d{1,3} \.
        \d{1,3} \.
        \d{1,3})/x;
    my $pat_quoted_field = qr/"((?:(?:(?:(?:    # It can be...
        [^"\\])* |  # ...zero or more characters not quote or backslash...
        (?:\\x[0-9a-fA-F][0-9a-fA-F])* | # ...a backslash quoted hexadecimal character...
        (?:\\.*)                         # ...or a backslash escape.
       ))*))"/x;
    my $parse_combined = qr/^       # Start at the beginning
         $pat_ip_address \s+        # IP address
         (\S+) \s+                  # Ident
         (\S+) \s+                  # Userid
         \[([^\]]*)\] \s+           # Date and time
         $pat_quoted_field \s+      # Request
         (\d+) \s+                  # Status
         (\-|[\d]+) \s+             # Length of reply or "-"
         $pat_quoted_field \s+      # Referer
         $pat_quoted_field          # User agent
         $                          # End at the end
       /x;

while (<>) {
    print if /$parse_combined/;
}
