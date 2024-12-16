#!/usr/bin/env bash

# Set Session Name
SESSION="Glutinum"
SESSIONEXISTS=$(tmux list-sessions | grep $SESSION)

# Only create tmux session if it doesn't already exist
if [ "$SESSIONEXISTS" = "" ]; then
    # Start New Session with our name
    tmux new-session -d -s $SESSION

    # Run 'echo "hello world"' in the first pane
    tmux send-keys -t $SESSION "./build.sh test specs --generate-only --watch" C-m

    # Split the window horizontally to create a second pane
    tmux split-window -h -t $SESSION

    # Run 'pwd' in the second pane
    tmux send-keys -t $SESSION:0.1 "./build.sh web --watch" C-m

    # Attach to the tmux session
    tmux attach -t $SESSION:0.0
fi

# Attach Session, on the Main window
tmux attach-session -t $SESSION:0.0
