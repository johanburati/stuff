#!/bin/sh
#
# List non-system users
#
# Copyright (C) 2011 Johan Burati <johan.burati@gmail.com>
# Code is licensed under GNU GPL license.
#
getent passwd | awk -F: '($3>=500) && ($3!=65534) { print $1}'
