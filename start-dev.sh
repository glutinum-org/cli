#!/usr/bin/env bash

# Set Session Name
SESSION="Glutinum"
SESSION_EXISTS=$(tmux list-sessions 2>/dev/null | grep $SESSION)

# Only create tmux session if it doesn't already exist
if [ "$SESSION_EXISTS" = "" ]; then
    # Start New Session with our name
    tmux new-session -d -s $SESSION

    starting_index=$(tmux show-options -v -g base-index)
    window_index=$((starting_index))
    left_pane_index=$((starting_index))
    right_pane_index=$((starting_index + 1))

    specs_pane_target="$SESSION:$window_index.$left_pane_index"
    web_pane_target="$SESSION:$window_index.$right_pane_index"

    # Split the window horizontally to create a second pane
    tmux split-window -h -t $specs_pane_target
    # Add label to panes
    tmux select-pane -t $specs_pane_target -T "Specs"
    tmux select-pane -t $web_pane_target -T "Web app"

    tmux send-keys -t $specs_pane_target "./build.sh test specs --generate-only --watch" C-m
    tmux send-keys -t $web_pane_target "./build.sh web --watch" C-m
fi

# Attach Session, on the Main window
tmux attach-session -t $SESSION
