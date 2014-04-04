
from bluetooth import *
import time
#--------------- MOTORI
import RPi.GPIO as io


io.setmode(io.BCM)

io.cleanup() 

in1_pin = 4
in2_pin = 17
pwm_pin= 23
in3_pin = 24
in4_pin = 25
pwm2_pin = 22
 
io.setup(in1_pin, io.OUT)
io.setup(in2_pin, io.OUT)
io.setup(pwm_pin, io.OUT)
io.setup(in3_pin, io.OUT)
io.setup(in4_pin, io.OUT)
io.setup(pwm2_pin, io.OUT)
p=io.PWM(pwm_pin, 500)
q=io.PWM(pwm2_pin, 500)

p.start(0)
p.ChangeDutyCycle(0)
q.start(0)
q.ChangeDutyCycle(0)

def changeSpeed(value1, value2 ):
	p.ChangeDutyCycle(value1)
	q.ChangeDutyCycle(value2)

 #motore 1
def motor1Orario():
    io.output(in1_pin, True)    
    io.output(in2_pin, False)
 
def motor1AntiOrario():
    io.output(in1_pin, False)
    io.output(in2_pin, True)
#motore 2
def motor2Orario():
	io.output(in3_pin,True)
	io.output(in4_pin,False)
	
def motor2AntiOrario():
	io.output(in3_pin,False)
	io.output(in4_pin,True)

def passaggioDati(data):
	line=data.split(",")
	direction=(int)(line[0])
	x_tmp=line[1]
	y_tmp=line[2]
	x=(int)(x_tmp)
	y=(int)(y_tmp)
	print "Messaggio ricevuto= "+str(x)+";"+str(y)
	if direction == 1:
		motor2Orario()
		motor1AntiOrario()
		controllerMotori(x,y)
	elif direction == 0:
		motor1Orario()
		motor2AntiOrario()
		controllerMotori(x,y)
	else:
		print "ERRORE: direzione ricevuta sconosciuta"
		
def controllerMotori(sx, dx):
			
		changeSpeed(sx,dx)
		

			
#----------------------

server_sock=BluetoothSocket( RFCOMM )
server_sock.bind(("",5))
server_sock.listen(1)

port = server_sock.getsockname()[1]

uuid = "94f39d29-7d6d-437d-973b-fba39e49d4ee"

advertise_service( server_sock, "SampleServer",
                   service_id = uuid,
                   service_classes = [ uuid, SERIAL_PORT_CLASS ],
                   profiles = [ SERIAL_PORT_PROFILE ],
#                   protocols = [ OBEX_UUID ]
                    )
                   
print("Waiting for connection on RFCOMM channel %d" % port)

client_sock, client_info = server_sock.accept()
print("Accepted connection from ", client_info)

try:
    while True:
        data = client_sock.recv(1024)
        if len(data) == 0: 
			break
        #print("received [%s]" % data)
        passaggioDati(data)
        
except IOError as e:
	print e
    

print("disconnected")

client_sock.close()
server_sock.close()
print("all done")


