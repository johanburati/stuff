#!/bin/sh
echo -- process with high priority --
ps -elf | perl -lane 'print if ($F[6] < 80)'
echo -- process with low priority --
ps -elf | perl -lane 'print if ($F[6] > 80)'
