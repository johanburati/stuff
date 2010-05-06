#!/bin/bash
#
# This script imports photos and videos from a camera connected via USB.
# The media are renamed according their creation date,
# then they are moved to a folder according to their new file name.
#
# filename format is: yyyymmdd_hhmmss.jpg (ex: 20100121_153312.jpg)
# folder structure is: basefolder/yyyy/mm/ (ex: basefolder/2010/01/)
#
# Johan Burati (johan.burati@gmail.com)
# Code is licensed under GNU GPL license.
#

#:NOTE: you probably want to change the five variables below
USB_CAMERA="/dev/sony-camera"
SRC_PHOTOS="/mnt/autofs/sony-camera/DCIM/101MSDCF"
SRC_VIDEOS="/mnt/autofs/sony-camera/MP_ROOT/101MNV01"
DST_PHOTOS="/mnt/doc/photos"
DST_VIDEOS="/mnt/doc/videos"

mkDir () {
# create a directory
    echo "   Create directory $1";
    mkdir -p $1
    if [ "$?" -ne 0 ]; then echo "   Error: Could not create directory ($1)"; exit 1; fi
}

mkTempDir () {
# create a tempory directory
    local TMP_BASEDIR=$(mktemp -d /home/$USER/tmp/cam-import-XXXXX)
    if [ ! -d $TMP_BASEDIR ]; then
       echo "   Error: Temp directory was not created ($TMP_BASEDIR)"; exit 1;
    fi
    echo "$TMP_BASEDIR"
}

rmDir () {
# ask for confirmation and delete a directory
    local RM_DIR=$1

    read -s -n1 -p "   Delete directory ($RM_DIR)? (y/n)" KEY; echo
    if [ "$KEY" == "y" ]; then
        if [ -d $RM_DIR ]; then
            local RM_FILE
            for RM_FILE in $RM_DIR/*; do
                echo $RM_FILE | egrep -q "(.jpg|.mp4)$"
                if [ "$?" -eq 0 ]; then
                    rm $RM_FILE
                    if [ "$?" -eq 0 ]; then
                        echo -n '.'
                    else
                        #echo "Warning(rmDir): Could not delete file ($RM_FILE) !";
                        echo -n 'x'
                    fi
                else
                    echo -n 'X'
                fi
            done
            echo
            rmdir "$RM_DIR"
            if [ "$?" -ne 0 ]; then echo "   Warning(rmDir): Could not delete directory ($RM_DIR) !"; fi
        else
            echo "   Warning: Could not find directory ($RM_DIR) !"
        fi
    fi
}


cpRenDir () {
# copy and rename files according to their creation date
    local SRC_CP=$1
    local DST_CP=$2
    local CP_COUNT=0
    local CP_FILE

    if [ ! -d $DST_CP ]; then echo "   Error(cpRenDir): Destination directory does not exist ($DST_CP) !"; exit 1; fi
    for CP_FILE in $SRC_CP; do
        local NEW_NAME=$(genNewName $CP_FILE)
        cp "$CP_FILE" "$DST_CP/$NEW_NAME"
        if [ "$?" -eq 0 ]; then
            echo -n '.'
            let CP_COUNT=$CP_COUNT+1
        else
            echo -n 'x'
        fi
    done
    echo
    _RET="$CP_COUNT"
}

cpDocDir () {
# copy date named file and put them in the according folder in the 'doc' directory
    local SRC_CP=$1
    local DST_CP=$2
    local CP_COUNT=0
    local CP_FILE

    if [ ! -d $DST_CP ]; then echo "   Error(cpDocDir): Destination directory does not exist !"; exit 1; fi
    for CP_FILE in $TMP_DIR/*.$LEXT; do
        local BASE_NAME=$(basename $CP_FILE)
        local YEAR=$(expr substr $BASE_NAME 1 4)
        local MONTH=$(expr substr $BASE_NAME 5 2)
        if [ ! -d $DST_DIR/$YEAR/$MONTH ]; then echo; mkDir "$DST_DIR/$YEAR/$MONTH"; fi
        if [ ! -f $DST_DIR/$YEAR/$MONTH/$BASE_NAME ]; then
            cp $CP_FILE $DST_DIR/$YEAR/$MONTH/
            if [ "$?" -eq 0 ]; then
                echo -n '.'
                let DST_COUNT=$DST_COUNT+1
            else
                echo -n 'x'
            fi
        else
            echo -n 's'
        fi
    done
    echo
    _RET="$CP_COUNT"
}

setDir () {
# set and verify the directories
    local FILES_TYPE=$1
    if [ $FILES_TYPE == "photos" ]; then
        SRC_DIR="$SRC_PHOTOS"
        DST_DIR="$DST_PHOTOS"
        EXT="JPG"
        LEXT="jpg"
    elif [ $FILES_TYPE == "videos" ]; then
        SRC_DIR="$SRC_VIDEOS"
        DST_DIR="$DST_VIDEOS"
        EXT="MP4"
        LEXT="mp4"
    else
        echo "   Error (camImport): unknown type specfied '$FILES_TYPE) !"
        exit 1
    fi
    TMP_DIR=$(mkTempDir)

    if [ ! -d $SRC_DIR ]; then echo "   Error: Can't find source directory ($SRV_DIR)"; exit 1; fi
    if [ ! -d $DST_DIR ]; then echo "   Error: Can't find destination directory ($DST_DIR)"; exit 1; fi
    if [ ! -d $TMP_DIR ]; then echo "   Error: Can't find temporary directory ($TMP_DIR)"; exit 1; fi
}

genNewName () {
# generate a new name for the file according to its creation date
    if [ -z $1 ]; then echo "   Error(genNewName): you must specify a parameter !"; exit 1; fi
    local FILE_NAME=$1

    local NEW_FILE_NAME=$(expr substr $(stat --format=%y $FILE_NAME  | tr ' ' '_' |  tr -d '\:-') 1 15).$LEXT
    # if the new_name already exist, increment
    if [ -f "$TMP_DIR/$NEW_FILE_NAME" ]; then
        CNT=1
        local TMP_FILE_NAME=$(expr substr $(stat --format=%y $FILE_NAME  | tr ' ' '_' |  tr -d '\:-') 1 15)_$(printf %02d $CNT).$LEXT
        while [ -f  "$TMP_DIR/$TMP_FILE_NAME" ]; do
            CNT=$(($CNT + 1))
            TMP_FILE_NAME=$(expr substr $(stat --format=%y $FILE_NAME  | tr ' ' '_' |  tr -d '\:-') 1 15)_$(printf %02d $CNT).$LEXT
        done
        NEW_FILE_NAME=$TMP_FILE_NAME
        if [ -f "$TMP_DIR/$NEW_FILE_NAME" ]; then
            echo
            echo "   Error: Cannot find non-duplicate name for $TMP_DIR/$FILE_NAME"
            exit 1
        fi
    fi
    echo "$NEW_FILE_NAME"
}

checkCam() {
# check if the camera is connected
    if [ ! -e $USB_CAMERA ]; then
        echo "   Error: camera is not connected !"
        exit 1
    fi
}

camImport () {
# import photos or videos from the camera
    local FILES_TYPE=$1

    #:NOTE: set the SRC_DIR,DST_DIR, TMP_DIR and EXT, LEXT variables
    setDir $FILES_TYPE

    #:NOTE: SRC_COUNT can be useful for a progress bar for the copy operation
    local SRC_COUNT=$(ls -1 $SRC_DIR/*.$EXT | grep -ic "$EXT$")

    #:NOTE: copy files from camera to TMP_DIR
    echo "++ Importing $FILES_TYPE from camera to '$TMP_DIR'"
    cpRenDir "$SRC_DIR/*.$EXT" "$TMP_DIR"
    TMP_COUNT=$_RET

    #:NOTE: move files from TMP_DIR to the documents folder
    echo "++ Moving $FILES_TYPE to doc folder '$DST_DIR'"
    cpDocDir "$TMP_DIR/*.$LEXT" "$DST_DIR"
    DST_COUNT=$_RET

    #:NOTE: verify the number of files copied
    echo "++ Checks/Cleanup"
    echo "++ Done, $SRC_COUNT/$TMP_COUNT/$DST_COUNT $FILES_TYPE copied."

    rmDir $TMP_DIR
}

main () {
    echo 'cam-import (1) - imports photos and videos from usb camera'
    checkCam

    echo "+ Imports Photos"
    camImport photos
    echo "+ Imports Videos"
    camImport videos

    gthumb $DST_DIR >/dev/null 2>&1 &
    echo '+ Completed !'
}

main
exit 0
