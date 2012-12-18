#!/bin/bash
set -x
SOURCEDIR=/data/doc
#BACKUPDIR=/mnt/autofs/usb-backup
BACKUPDIR=/media/NDATA
RSYNCLOG=${HOME}/tmp/usb-backup.log
if [ -d ${SOURCEDIR} ] && [ -d ${BACKUPDIR} ]; then
    rsync -auv --delete ${SOURCEDIR} ${BACKUPDIR} 2>&1 > ${RSYNCLOG}
    if [ "$?" -eq "0" ]; then
        logger "${SOURCEDIR} backup to USB device was successful"
        exit 0
    else
        logger "${SOURCEDIR} backup to USB device failed"
        exit 1
    fi
else
    if [ ! -d ${SOURCEDIR} ]; then
        echo "Error: \'${SOURCEDIR}\' does not exit" >&2
    fi
    if [ ! -d ${BACKUPDIR} ]; then
        echo "Error: \'${BACKUPDIR}\' does not exit" >&2
    fi
    exit 1
fi
exit 0
