import bpy
import os

def find_armature():
    for obj in bpy.context.selected_objects:
        if obj.type == 'ARMATURE':
            return obj
    for obj in bpy.data.objects:
        if obj.type == 'ARMATURE':
            return obj
    return None

armature = find_armature()
if armature is None:
    raise RuntimeError("No armature object found")

mesh_objects = [obj for obj in bpy.data.objects if obj.type == 'MESH' and obj.find_armature() == armature]

forced_deform_bones = {}
for bone in armature.data.bones:
    if bone.parent is None or "root" in bone.name.lower():
        forced_deform_bones[bone.name] = bone.use_deform
        bone.use_deform = True

bpy.ops.object.select_all(action='DESELECT')
armature.select_set(True)
for obj in mesh_objects:
    obj.select_set(True)
bpy.context.view_layer.objects.active = armature

if bpy.data.filepath:
    export_dir = os.path.dirname(bpy.data.filepath)
else:
    export_dir = os.path.expanduser("~")

output_name = mesh_objects[0].name if mesh_objects else armature.name

export_path = os.path.join(export_dir, output_name + ".glb")

try:
    bpy.ops.export_scene.gltf(
        filepath=export_path,
        use_selection=True,
        export_format='GLB',
        export_materials='NONE',
        export_yup=True,
        export_def_bones=True,
        export_skins=True,
        export_animations=True,
        export_force_sampling=True,
        export_apply=False,
    )
finally:
    for name, state in forced_deform_bones.items():
        armature.data.bones[name].use_deform = state

print("Exported:", export_path)