## Project Description

The goal of this project is to create an implementation of a device that can write files to a directory and then to design a write scheduling system for a set of devices. The write scheduler system consist of an interface named WriteScheduler and contains two concrete implementations of it. The first is a round robin scheduler and the second is a write scheduler that uses some combination of pending writes, total writes, or total bytes written to optimize the writes.
