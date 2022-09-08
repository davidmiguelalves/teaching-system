import argparse, sys
import os
import shutil
import random

def copy_file(src_file, dest_file):

    # verify source file
    if not os.path.exists(src_file):
        print('File:', src_file, 'don\'t exist!')
        sys.exit('Error')

    # copy file
    shutil.copyfile(src_file, dest_file)

def create_essential_directories(dataset):

    # get current dir
    current_directory = os.getcwd()

    # data directory
    data_directory = os.path.abspath(os.path.join(current_directory, dataset, 'data'))
    os.makedirs(data_directory, exist_ok=True)

    # train directory
    train_directory = os.path.abspath(os.path.join(current_directory, dataset, 'train'))
    os.makedirs(train_directory, exist_ok=True)

def create_classes_file(dataset):

    # get current dir
    current_directory = os.getcwd()

    # source file
    src_file = os.path.abspath(os.path.join(current_directory, dataset, 'images', 'classes.txt'))

    # destiny file
    dest_file = os.path.abspath(os.path.join(current_directory, dataset, 'data', 'classes.names'))

    # copy file
    copy_file(src_file,dest_file)

def create_train_test_file(dataset):

    # get current dir
    current_directory = os.getcwd()

    # get all jpg files
    path = dataset + '/images/'

    all_files_aux = []

    for current_dir, dirs, files in os.walk(path):
        for file in files:
            if file.endswith('.jpg'):
                all_files_aux.append(path + file + '\n')

    # *0.75 -> deixar os primeiros 75% (150 imagens)
    # *0.50 -> deixar os primeiros 75% (100 imagens)
    # *0.25 -> deixar os primeiros 75% (50 imagens)
    #[:int(len(all_files_aux) * 0.75)]

    # shuffle the array
    random.shuffle(all_files_aux)
    all_files = all_files_aux

    # get lenght 30% of elements from the list
    lenght = int(len(all_files) * 0.3)
    
    # get test files - 30% of elements
    test_files = all_files[:lenght]

    # get test file name
    test_file = os.path.abspath(os.path.join(current_directory, dataset, 'data', 'test.txt'))
    
    # create test file
    with open(test_file, 'w+') as f:
        for line in test_files:
            f.write(line)

    # remove 30% of the first elements of the list
    train_files = all_files[lenght:]
    
    # get train file name
    train_file = os.path.abspath(os.path.join(current_directory, dataset, 'data', 'train.txt'))
    
    # create train file
    with open(train_file, 'w+') as f:
        for line in train_files:
            f.write(line)

def create_data_file(dataset):

    # get current dir
    current_directory = os.getcwd()
    
    # get data file name
    data_file = os.path.abspath(os.path.join(current_directory, dataset, 'data', 'data.data'))
    
    # get classes file name
    classes_file = os.path.abspath(os.path.join(current_directory, dataset, 'data', 'classes.names'))

    classes = sum(1 for line in open(classes_file))

    # create test file
    with open(data_file, 'w+') as f:
        f.write('classes = '+ str(classes) + '\n')
        f.write('train = ' + dataset + '/data/train.txt\n')
        f.write('valid = ' + dataset + '/data/test.txt\n')
        f.write('names = ' + dataset + '/data/classes.names\n')
        f.write('backup = ' + dataset + '/train/\n')

def create_config_file(dataset):

    # get current dir
    current_directory = os.getcwd()

    # source file
    src_file = os.path.join(current_directory, 'darknet', 'cfg', 'yolov4-tiny-custom.cfg')

    # destiny file
    dest_file = os.path.join(current_directory, dataset, 'data', 'config.config')

    # copy file
    copy_file(src_file,dest_file)

    # get classes file name
    classes_file = os.path.abspath(os.path.join(current_directory, dataset, 'data', 'classes.names'))

    classes = sum(1 for line in open(classes_file))

    # generate content
    lines = []
    max_batches = int(2000 * classes)
    with open(dest_file) as file:
        for line in file:
            line = line.strip().replace(" ", "")
            if line == 'batch=64':
                lines.append('batch=32')
            elif line == 'subdivisions=1':
                lines.append('subdivisions=12')
            elif line == 'max_batches=500200':
                lines.append('max_batches=' + str(max_batches))
            elif line == 'steps=400000,450000':
                lines.append('steps=' + str(int(0.8 * max_batches)) + ',' + str(int(0.9 * max_batches)))
            elif line == 'classes=80':
                lines.append('classes=' + str(classes))
            elif line == 'filters=255':
                lines.append('filters=' + str(8 * classes))
            else:
                lines.append(line)
    
    # create test file
    with open(dest_file, 'w+') as f:
        for line in lines:
            f.write(line + '\n')

if __name__ == '__main__':

    parser=argparse.ArgumentParser()
    parser.add_argument('--dataset', help='Dataset Name')
    args=parser.parse_args()
    
    if len(sys.argv)<=1:
        parser.print_help()
        sys.exit()

    # dataset name
    dataset = args.dataset

    # change directory to previous of script
    os.chdir(os.path.dirname(os.path.realpath(__file__)) + '/../')

    # verify if folder exists
    if not os.path.exists(dataset):
        print('Folder:', dataset, 'don\'t exist!')
        sys.exit('Error')
        
    # create directories: data and train
    create_essential_directories(dataset)

    # create classes.names file
    create_classes_file(dataset)

    # create train.txt and test.txt files
    create_train_test_file(dataset)
    
    # create data.data file
    create_data_file(dataset)

    # create config.config file
    create_config_file(dataset)

