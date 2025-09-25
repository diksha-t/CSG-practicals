import maya.cmds as cmds

def create_rigid_debris(num_pieces=25,
                        area_size=6.0,
                        height=6.0,
                        min_scale=0.2,
                        max_scale=1.0,
                        mass_range=(0.5, 3.0),
                        use_explosion=True,
                        explosion_magnitude=80.0,
                        explosion_maxdist=10.0):
    # Create ground
    if not cmds.objExists("debris_ground"):
        ground = cmds.polyPlane(w=30, h=30, sx=1, sy=1, n="debris_ground")[0]
        cmds.move(0, 0, 0, ground)
    else:
        ground = "debris_ground"
    # Make ground passive rigid body
    try:
        cmds.rigidBody(ground, active=False)  # passive
    except Exception:
        pass

    debris_list = []
    import random
    for i in range(num_pieces):
        sx = random.uniform(min_scale, max_scale)
        sy = random.uniform(min_scale, max_scale)
        sz = random.uniform(min_scale, max_scale)
        name = cmds.polyCube(w=sx, h=sy, d=sz, n="debris_%02d" % (i+1))[0]
        # random position in XY plane above ground
        x = random.uniform(-area_size, area_size)
        z = random.uniform(-area_size, area_size)
        y = random.uniform(height*0.6, height)
        cmds.move(x, y, z, name)
        # slight random rotation
        cmds.rotate(random.uniform(0,360),
                    random.uniform(0,360),
                    random.uniform(0,360), name)
        # create active rigid body (mass randomized)
        m = random.uniform(mass_range[0], mass_range[1])
        cmds.rigidBody(name, active=True, mass=m, bounciness=0.1, friction=0.6)
        debris_list.append(name)

    # Optional radial "explosion" field at origin
    if use_explosion:
        # create radial field at origin
        rfield = cmds.radial(pos=(0, height*0.5, 0), m=explosion_magnitude, mxd=explosion_maxdist)[0]
        # you might want to key its magnitude to simulate a single blast (or just leave it on)
        # connect field to all rigid bodies (fields affect dynamics automatically if within range)
        cmds.select(debris_list, rfield)
    print("Created %d rigid debris pieces. Play the timeline to simulate." % len(debris_list))

# Run with defaults
create_rigid_debris()
