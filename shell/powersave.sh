#!/bin/bash
#
# powersafe - turn power saving features on/off
#
# Copyright (C) 2011 Johan Burati <johan.burati@gmail.com>
# Code is licensed under GNU GPL license.
#
#!/bin/bash

[ -d /sys/class/scsi_host/ ] || exit 0

. /usr/lib/pm-utils/functions

powersave_on()
{
   echo "- SATA link power management"
   echo min_power | sudo tee -a /sys/class/scsi_host/host0/link_power_management_policy
   echo "- VM"
   echo 1500 | sudo tee -a /proc/sys/vm/dirty_writeback_centisecs
   echo "- HDA-Intel power saving mode"
   echo 10 | sudo tee -a /sys/module/snd_hda_intel/parameters/power_save
}

powersave_off()
{
   echo "- SATA link power management"
   echo max_performance | sudo tee -a /sys/class/scsi_host/host0/link_power_management_policy
   echo "- VM"
   echo 500 | sudo tee -a /proc/sys/vm/dirty_writeback_centisecs
   echo "- HDA-Intel power saving mode"
   echo 0 | sudo tee -a /sys/module/snd_hda_intel/parameters/power_save
}

powersave_status()
{
   echo -n "- SATA link power management: "
   cat /sys/class/scsi_host/host0/link_power_management_policy
   echo -n "- VM writeback time: "
   cat /proc/sys/vm/dirty_writeback_centisecs
   echo -n "- HDA-Intel power saving mode: "
   cat /sys/module/snd_hda_intel/parameters/power_save
}

case "$1" in
   status)
      powersave_status
      ;;
   off)
      powersave_off
      ;;
   on)
      powersave_on
      ;;
   *)
      ;;
esac


