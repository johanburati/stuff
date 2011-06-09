#!/bin/bash
#
# This script is a replacement for the runme.php script
# that comes with the sheevaplug-installer-v1.0.tar.gz
# since the php script does not work on fedora 12 x86_64.
#
# Johan Burati (johan.burati@gmail.com)
# Code is licensed under GNU GPL license.
#

#:NOTE: update the var below with the MAC address at the back of your sheevaplug
ETHADDR="00:50:43:01:C4:CF"

set_uboot_env() {
# this routine adds parameters in a text file (uboot-dflt.txt and uboot-$TARGET-custom.txt)
# to the uboot-env.bin file in the current directory, ignore comment and empty lines
    INFILE=$1
    if [ ! -f $INFILE ]; then
        echo "  Error: Input file is missing ($INFILE) !"
        exit 1
    fi

    echo "++ Reading $INFILE"
    cat $INFILE | while read LINE; do
        echo $LINE | egrep -q '(^\s*$|^#)'
        if [ "$?" -ne 0 ]; then
            CMD="$FW_SETENV $LINE"
            echo "  $CMD"
            $CMD
        fi
    done
}

mustbe_root() {
# must run the script as root
    if [[ $EUID -ne 0 ]]; then
        echo "  Error: This script must be run as root !"
        exit 1
    fi
}

set_install_target() {
# must have on parameter, either nand or mmc
    if [ -z $TARGET ] || [[ $TARGET != "nand" && $TARGET != "mmc" ]]; then
        echo "Usage: $0 [ nand | mmc ]"
        exit 1
    fi
}

load_ftdi_sio() {
# load the ftdi_sio module, so that the shevaplug console show up as /dev/ttyUSB0
    echo "+ Load USB FTDI Serial Converters Driver (ftdi_sio)"
    modprobe ftdi_sio vendor=0x9e88 product=0x9e8f
    if [ "$?" -ne 0 ]; then
        echo "  Error: Could not load the ftdi_sio module !"
        exit 1
    fi
}

reset_uboot_env () {
# remove the last uboot-env.bin and create an empty one
    if [ -f uboot-env.bin ]; then
        rm uboot-env.bin
        if [ "$?" -ne 0 ]; then echo "Error: Could not delete uboot-env.bin"; exit 1; fi
    fi
    touch uboot-env.bin
}

update_uboot_env () {
# update the uboot-dflt.txt and uboot-$TARGET-custom.txt with sheevaplug MAC address
    if [ ! -f uboot/uboot-env/uboot-dflt.txt ]; then
        echo "  Error: Could not find file uboot/uboot-env/uboot-dflt.txt!"; exit 1;
    fi
    if [ ! -f uboot/uboot-env/uboot-$TARGET-custom.txt ]; then
        echo "  Error: Could not find file uboot/uboot-env/uboot-$TARGET-custom.txt !"; exit 1;
    fi
    sed -i "/^ethaddr/c ethaddr $ETHADDR" uboot/uboot-env/uboot-dflt.txt
    sed -i "/^ethaddr/c ethaddr $ETHADDR" uboot/uboot-env/uboot-$TARGET-custom.txt
}

move_uboot_env() {
# move the uboot-env.bin to the uboot directory
    if [ -f uboot/uboot-env.bin ]; then
        rm uboot/uboot-env.bin
        if [ "$?" -ne 0 ]; then echo "Error: Could not delete uboot/uboot-env.bin"; exit 1; fi
    fi
    mv uboot-env.bin uboot/uboot-env.bin
}

burn_uboot() {
# burn the uboot
    echo "+ Burning uboot and environment variables, this will take a few minutes..."
    cd uboot
    if [ "$?" -ne 0 ]; then echo "  Error: Did not find 'uboot' directory !"; exit 1; fi
    OPENOCD_CMD='openocd/openocd'
    if [ "$?" -ne 0 ]; then echo "  Error: Please install openocd on your system";  exit 1; fi
    if [ ! -f openocd/config/board/sheevaplug.cfg ]; then echo "  Error: Could not find sheevaplug.cfg";  exit 1; fi

    OPENOCD_CMD="$OPENOCD_CMD
#     -d3
     -f openocd/config/board/sheevaplug.cfg
     -s openocd/config/
     -c init
     -c sheevaplug_reflash_uboot_env
     -c sheevaplug_reflash_uboot
     -c exit"
    # Burn u-boot environment variables first, then u-boot
    $OPENOCD_CMD
    RC="$?"
    # remove the uboot-env.bin file
    rm uboot-env.bin
    cd ..
    if [ $RC -ne 0 ]; then echo "  Error:  Burn process failed !"; exit 1; fi
}

beep() {
# play a sound if possible
    if [ -f /usr/bin/play ] && [ -f /usr/share/sounds/pop.wav ]; then
        play -q /usr/share/sounds/pop.wav
    else
        echo "  Done !"
    fi
}

main() {
    FW_SETENV="uboot/uboot-env/fw_setenv"
    mustbe_root
    set_install_target
    load_ftdi_sio

    echo "+ Update environment variables file image (uboot/uboot-env.bin)";
    reset_uboot_env
    update_uboot_env
    set_uboot_env uboot/uboot-env/uboot-dflt.txt
    set_uboot_env uboot/uboot-env/uboot-$TARGET-custom.txt
    move_uboot_env

    burn_uboot

    echo "+ U-boot should be up and running now. Open your console ..."
    beep
}

TARGET=$1
main
exit 0
