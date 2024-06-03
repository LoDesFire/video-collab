#!/usr/bin/env bash

set -e

max_bitrate_ratio=1.07          # maximum accepted bitrate fluctuations
rate_monitor_buffer_ratio=1.5   # maximum buffer size between bitrate conformance checks

#########################################################################

source="${1}"
target="${2}"
ffmpeg="${3}"
renditions=(${4})
if [[ ! "${target}" ]]; then
  target="${source##*/}" # leave only last component of path
  target="${target%.*}"  # strip extension
fi
mkdir -p ${target}


key_frames_interval="$(echo `ffprobe ${source} 2>&1 | grep -oE '[[:digit:]]+(.[[:digit:]]+)? fps' | grep -oE '[[:digit:]]+(.[[:digit:]]+)?'`*2 | bc || echo '')"
key_frames_interval=${key_frames_interval:-50}
key_frames_interval=$(echo `printf "%.1f\n" $(bc -l <<<"$key_frames_interval/10")`*10 | bc) # round
key_frames_interval=${key_frames_interval%.*} # truncate to integer

# static parameters that are similar for all renditions

precmd_static="-c:v h264 -profile:v main -crf 20 -sc_threshold 0"
precmd_static+=" -g ${key_frames_interval} -keyint_min ${key_frames_interval}"

#postcmd=" -hls_flags single_file -hls_playlist_type vod -master_pl_name master.m3u8"
postcmd=" -hls_playlist_type vod -master_pl_name master.m3u8"
postcmd+=" -hls_segment_filename ${target}/qual%v/mov_%03d.ts ${target}/qual%v/index.m3u8"

# misc params
misc_params="-hide_banner -y"

precmd=" "
stream_map=""
cmd=""
index="0"
for rendition in "${renditions[@]}"; do
  precmd+="-map 0:v:0 -map 0:a:0 "
  stream_map+=" v:${index},a:${index}"

  # drop extraneous spaces
  rendition="${rendition/[[:space:]]+/ }"

  # rendition fields
  resolution="$(echo ${rendition} | cut -d '_' -f 1)"
  mkdir -p ${target}/qual${index} 

  bitrate="$(echo ${rendition} | cut -d '_' -f 2)"
  audiorate="$(echo ${rendition} | cut -d '_' -f 3)"

  # calculated fields
  width="$(echo ${resolution} | grep -oE '^[[:digit:]]+')"
  height="$(echo ${resolution} | grep -oE '[[:digit:]]+$')"
  maxrate="$(echo "`echo ${bitrate} | grep -oE '[[:digit:]]+'`*${max_bitrate_ratio}" | bc)"
  bufsize="$(echo "`echo ${bitrate} | grep -oE '[[:digit:]]+'`*${rate_monitor_buffer_ratio}" | bc)"
#  bandwidth="$(echo ${bitrate} | grep -oE '[[:digit:]]+')000"
  
  cmd+=" -filter:v:${index} scale=w=${width}:h=${height}:force_original_aspect_ratio=decrease:force_divisible_by=2"
  cmd+=" -b:v:${index} ${bitrate} -maxrate:v:${index} ${maxrate%.*}k -bufsize:v:${index} ${bufsize%.*}k -b:a:${index} ${audiorate}"
  ((index=index+1))
done
precmd+=${precmd_static}

${ffmpeg} ${misc_params} -i ${source} ${precmd} ${cmd} -var_stream_map "${stream_map}" ${postcmd}
