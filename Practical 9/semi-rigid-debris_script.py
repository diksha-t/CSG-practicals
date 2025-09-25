import maya.cmds as cmds
import random

def create_semi_rigid_debris(num_pieces=20,
                             area_size=6.0,
                             height=6.0,
                             min_scale=0.3,
                             max_scale=1.2):
    # Create ground plane
    if not cmds.objExists("debris_ground"):
        ground = cmds.polyPlane(w=30, h=30, sx=1, sy=1, n="debris_ground")[0]
        cmds.move(0, 0, 0, ground)
    else:
        ground = "debris_ground"
    
    # Make ground passive rigid body
    try:
        cmds.rigidBody(ground, active=False)
    except Exception:
        pass

    debris_list = []
    for i in range(num_pieces):
        # Random shape (cube or sphere)
        if random.random() > 0.5:
            name = cmds.polyCube()[0]
        else:
            name = cmds.polySphere()[0]
        
        # Random scale
        sx = random.uniform(min_scale, max_scale)
        sy = random.uniform(min_scale, max_scale)
        sz = random.uniform(min_scale, max_scale)
        cmds.scale(sx, sy, sz, name)

        # Random position above ground
        x = random.uniform(-area_size, area_size)
        z = random.uniform(-area_size, area_size)
        y = random.uniform(height*0.5, height)
        cmds.move(x, y, z, name)

        # Create active rigid body
        rb = cmds.rigidBody(name, active=True, mass=random.uniform(0.5, 2.0))[0]
        
        # Add semi-rigid effect → low bounciness + some damping
        cmds.setAttr(rb + ".bounciness", 0.2)   # not too rigid
        cmds.setAttr(rb + ".damping", 0.3)      # simulates slight energy loss
        cmds.setAttr(rb + ".friction", 0.6)

        debris_list.append(name)

    # Add a turbulence field for extra semi-rigid jitter
    turb = cmds.turbulence(pos=(0, height/2, 0), m=50, att=10)[0]
    cmds.connectDynamic(debris_list, f=turb)

    print("Created {} semi-rigid debris pieces. Play the timeline to simulate.".format(len(debris_list)))


# Run the function
create_semi_rigid_debris()
