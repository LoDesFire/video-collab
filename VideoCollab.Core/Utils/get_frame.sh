#!/usr/bin/env bash

source="${1}"
target="${2}"

ffmpeg -i "$source" -y -ss 00:00:00 -vframes 1 "$target"