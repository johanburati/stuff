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
usb_camera="/dev/sony-camera"
#src_root="/mnt/autofs/sony-camera"
src_root="/media/disk"
src_photos="$src_root/DCIM/101MSDCF"
src_videos="$src_root/MP_ROOT/101MNV01"
dst_photos="/mnt/doc/photos"
dst_videos="/mnt/doc/videos"

dir_create () {
# create a directory
    echo "   Create directory $1";
    mkdir -p $1
    if [ "$?" -ne 0 ]; then echo "   Error: Could not create directory ($1)"; exit 1; fi
}

dir_create_temp () {
# create a tempory directory
    local tmp_basedir=$(mktemp -d /home/$USER/tmp/cam-import-XXXXX)
    if [ ! -d $tmp_basedir ]; then
       echo "   Error: Temp directory was not created ($tmp_basedir)"; exit 1;
    fi
    echo "$tmp_basedir"
}

dir_destroy () {
# ask for confirmation and delete a directory
    local rm_dir=$1

    read -s -n1 -p "   Delete directory ($rm_dir)? (y/n)" key; echo
    if [ "$key" == "y" ]; then
        if [ -d $rm_dir ]; then
            local rm_file
            for rm_file in $rm_dir/*; do
                echo $rm_file | egrep -q "(.jpg|.mp4)$"
                if [ "$?" -eq 0 ]; then
                    rm $rm_file
                    if [ "$?" -eq 0 ]; then
                        echo -n '.'
                    else
                        #echo "Warning(dir_destroy): Could not delete file ($rm_file) !";
                        echo -n 'x'
                    fi
                else
                    echo -n 'X'
                fi
            done
            echo
            rmdir "$rm_dir"
            if [ "$?" -ne 0 ]; then echo "   Warning(dir_destroy): Could not delete directory ($rm_dir) !"; fi
        else
            echo "   Warning: Could not find directory ($rm_dir) !"
        fi
    fi
}

dir_copy_and_rename () {
# copy and rename files according to their creation date
    local src_cp=$1
    local dst_cp=$2
    local cp_count=0
    local cp_file

    if [ ! -d $dst_cp ]; then echo "   Error(dir_copy_and_rename): Destination directory does not exist ($dst_cp) !"; exit 1; fi
    for cp_file in $src_cp; do
        local new_name=$(filename_generate $cp_file)
        cp "$cp_file" "$dst_cp/$new_name"
        if [ "$?" -eq 0 ]; then
            echo -n '.'
            let cp_count=$cp_count+1
        else
            echo -n 'x'
        fi
    done
    echo
    _ret="$cp_count"
}

dir_copy_to_doc () {
# copy date named file and put them in the according folder in the 'doc' directory
    local src_cp=$1
    local dst_cp=$2
    local cp_count=0
    local cp_file

    if [ ! -d $dst_cp ]; then echo "   Error(dir_copy_to_doc): Destination directory does not exist !"; exit 1; fi
    for cp_file in $tmp_dir/*.$lext; do
        local base_name=$(basename $cp_file)
        local year=$(expr substr $base_name 1 4)
        local month=$(expr substr $base_name 5 2)
        if [ ! -d $dst_dir/$year/$month ]; then echo; dir_create "$dst_dir/$year/$month"; fi
        if [ ! -f $dst_dir/$year/$month/$base_name ]; then
            cp $cp_file $dst_dir/$year/$month/
            if [ "$?" -eq 0 ]; then
                echo -n '.'
                let cp_count=$cp_count+1
            else
                echo -n 'x'
            fi
        else
            echo -n 's'
        fi
    done
    echo
    _ret="$cp_count"
}

directories_set () {
# set and verify the directories
    local files_type=$1
    if [ $files_type == "photos" ]; then
        src_dir="$src_photos"
        dst_dir="$dst_photos"
        ext="JPG"
        lext="jpg"
    elif [ $files_type == "videos" ]; then
        src_dir="$src_videos"
        dst_dir="$dst_videos"
        ext="MP4"
        lext="mp4"
    else
        echo "   Error (cam_import): unknown type specfied '$files_type) !"
        exit 1
    fi
    tmp_dir=$(dir_create_temp)

    if [ ! -d $src_dir ]; then echo "   Error: Can't find source directory ($SRV_DIR)"; exit 1; fi
    if [ ! -d $dst_dir ]; then echo "   Error: Can't find destination directory ($dst_dir)"; exit 1; fi
    if [ ! -d $tmp_dir ]; then echo "   Error: Can't find temporary directory ($tmp_dir)"; exit 1; fi
}

filename_generate () {
# generate a new name for the file according to its creation date
    if [ -z $1 ]; then echo "   Error(filename_generate): you must specify a parameter !"; exit 1; fi
    local file_name=$1

    local new_file_name=$(expr substr $(stat --format=%y $file_name  | tr ' ' '_' |  tr -d '\:-') 1 15).$lext
    # if the new_name already exist, increment
    if [ -f "$tmp_dir/$new_file_name" ]; then
        cnt=1
        local tmp_file_name=$(expr substr $(stat --format=%y $file_name  | tr ' ' '_' |  tr -d '\:-') 1 15)_$(printf %02d $cnt).$lext
        while [ -f  "$tmp_dir/$tmp_file_name" ]; do
            cnt=$(($cnt + 1))
            tmp_file_name=$(expr substr $(stat --format=%y $file_name  | tr ' ' '_' |  tr -d '\:-') 1 15)_$(printf %02d $cnt).$lext
        done
        new_file_name=$tmp_file_name
        if [ -f "$tmp_dir/$new_file_name" ]; then
            echo
            echo "   Error: Cannot find non-duplicate name for $tmp_dir/$file_name"
            exit 1
        fi
    fi
    echo "$new_file_name"
}

cam_is_connected() {
# check if the camera is connected
#    if [ ! -e $usb_camera ]; then
    if [ ! -e $src_photos ]; then
        echo "   Error: camera is not connected !"
        exit 1
    fi
}

cam_import () {
# import photos or videos from the camera
    local files_type=$1

    #:NOTE: set the src_dir,dst_dir, tmp_dir and ext, lext variables
    directories_set $files_type

    #:NOTE: src_count can be useful for a progress bar for the copy operation
    local src_count=$(ls -1 $src_dir/*.$ext 2>/dev/null| grep -ic "$ext$")
    if [[ "$src_count" > 0 ]]; then
        #:NOTE: copy files from camera to tmp_dir
        echo "++ Importing $files_type from camera to '$tmp_dir'"
        dir_copy_and_rename "$src_dir/*.$ext" "$tmp_dir"
        tmp_count=$_ret

        #:NOTE: move files from tmp_dir to the documents folder
        echo "++ Moving $files_type to doc folder '$dst_dir'"
        dir_copy_to_doc "$tmp_dir/*.$lext" "$dst_dir"
        dst_count=$_ret

        #:NOTE: verify the number of files copied
        echo "++ Checks/Cleanup"
        echo "++ Done, $src_count/$tmp_count/$dst_count $files_type copied."

        dir_destroy $tmp_dir
    else
        echo "++ No file found !"
    fi
}

main () {
    echo 'cam-import (1) - imports photos and videos from usb camera'
    cam_is_connected

    echo "+ Imports Photos"
    cam_import photos

    read -s -n1 -p "   Do you want to import the videos ? (y/n)" key; echo
    if [ "$key" == "y" ]; then
        echo "+ Imports Videos"
        cam_import videos
    fi

    #gthumb $dst_photos >/dev/null 2>&1 &
    # Start up shotwell
    # At this time there is no command line parameter to import a folder so
    # we need to import the folder manually
    shotwell
    echo '+ Completed !'
}

main
exit 0
