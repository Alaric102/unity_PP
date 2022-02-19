from dt_apriltags import Detector
import pyrealsense2 as rs
import numpy as np
import cv2
import math
# import socket_class as sc
import time
import socket
import pickle
import struct

def send_data(conn, payload, data_id=0):
    serialized_payload = pickle.dumps(payload)
    conn.sendall(serialized_payload)

send_dict = {}
send_dict.setdefault('Box', [0.0, 0.0, 0.0])
send_dict.setdefault('Office_chair', [0.0, 0.0, 0.0])
send_dict.setdefault('Soccer_ball', [0.0, 0.0, 0.0])
send_dict.setdefault('Wood', [0.0, 0.0, 0.0])
send_dict.setdefault('Other', [0.0, 0.0, 0.0])

conn = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
conn.connect(('10.16.112.72', 12345))
send_data(conn, send_dict)

at_detector = Detector(families='tag36h11',
                       nthreads=1,
                       quad_decimate=1.0,
                       quad_sigma=0.0,
                       refine_edges=1,
                       decode_sharpening=0.25,
                       debug=0)

# Intrinsic camera matrix for the raw (distorted) images.
#       [fx  0 cx]
# mtx = [ 0 fy cy]
#       [ 0  0  1]

mtx = [378.926, 0, 325.999, 0, 378.926, 231.822, 0, 0, 1]
cameraMatrix = np.array(mtx).reshape((3,3))
camera_params = ( cameraMatrix[0,0], cameraMatrix[1,1], cameraMatrix[0,2], cameraMatrix[1,2] )

pipeline = rs.pipeline()
config = rs.config()

# Get device product line for setting a supporting resolution
pipeline_wrapper = rs.pipeline_wrapper(pipeline)
pipeline_profile = config.resolve(pipeline_wrapper)
device = pipeline_profile.get_device()
device_product_line = str(device.get_info(rs.camera_info.product_line))

found_rgb = False
for s in device.sensors:
    if s.get_info(rs.camera_info.name) == 'RGB Camera':
        found_rgb = True
        break
if not found_rgb:
    print("The demo requires Depth camera with Color sensor")
    exit(0)

config.enable_stream(rs.stream.color, 640, 480, rs.format.bgr8, 30)


if __name__ == '__main__':
    pipeline.start(config)
    while cv2.waitKey(1) != 0x1b:
        frames = pipeline.wait_for_frames()
        # depth_frame = frames.get_depth_frame()
        color_frame = frames.get_color_frame()

        # ret, img = cam.read()
        img = np.asanyarray(color_frame.get_data())
        greys = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
        tags = at_detector.detect(greys, estimate_tag_pose=True, camera_params=camera_params, tag_size=0.167)
        print(tags)
        
        message_tosend = ""
        if tags:
            for tag in tags:
                for idx in range(len(tag.corners)):
                    cv2.line(img, tuple(tag.corners[idx-1, :].astype(int)), tuple(tag.corners[idx, :].astype(int)), (0, 255, 0))

                cv2.putText(img, str(tag.tag_id),
                            org=(tag.corners[0, 0].astype(int)+10,tag.corners[0, 1].astype(int)+10),
                            fontFace=cv2.FONT_HERSHEY_SIMPLEX,
                            fontScale=0.8,
                            color=(0, 0, 255))

                # if tag.tag_id == 0:
                #     print('Box: ', tag.pose_t[0][0], tag.pose_t[1][0], tag.pose_t[2][0])
                #     send_dict['Box'] = [tag.pose_t[0][0], tag.pose_t[1][0], tag.pose_t[2][0]]
                #     send_data(conn, send_dict)

                # elif tag.tag_id == 1:
                #     print('Office_chair: ', tag.pose_t[0][0], tag.pose_t[1][0], tag.pose_t[2][0])
                #     send_dict['Office_chair'] = [tag.pose_t[0][0], tag.pose_t[1][0], tag.pose_t[2][0]]
                #     send_data(conn, send_dict)
                    
                # elif tag.tag_id == 2:
                #     print('Soccer_ball: ', tag.pose_t[0][0], tag.pose_t[1][0], tag.pose_t[2][0])
                #     send_dict['Soccer_ball'] = [tag.pose_t[0][0], tag.pose_t[1][0], tag.pose_t[2][0]]
                #     send_data(conn, send_dict)
                
                # elif tag.tag_id == 3:
                #     print('Wood: ', tag.pose_t[0][0], tag.pose_t[1][0], tag.pose_t[2][0])
                #     send_dict['Wood'] = [tag.pose_t[0][0], tag.pose_t[1][0], tag.pose_t[2][0]]
                #     send_data(conn, send_dict)
                
                # elif tag.tag_id == 4:
                #     print('Other: ', tag.pose_t[0][0], tag.pose_t[1][0], tag.pose_t[2][0])
                #     send_dict['Other'] = [tag.pose_t[0][0], tag.pose_t[1][0], tag.pose_t[2][0]]
                #     send_data(conn, send_dict)
                message_tosend += tag.tag_id + ','
                message_tosend += tag.pose_t[0][0] + ','
                message_tosend += tag.pose_t[1][0] + ','
                message_tosend += tag.pose_t[2][0]

        else:
            print("No tags")
            # send_dict['Box'] = [0.0, 0.0, 0.0]
            # send_dict['Office_chair'] = [0.0, 0.0, 0.0]
            # send_dict['Soccer_ball'] = [0.0, 0.0, 0.0]
            # send_dict['Wood'] = [0.0, 0.0, 0.0]
            # send_dict['Other'] = [0.0, 0.0, 0.0]
            # send_data(conn, send_dict)
            
            message_tosend = "-1"
        
        send_data(conn, message_tosend)
        
        cv2.imshow('One', img)
    cv2.destroyAllWindows()
