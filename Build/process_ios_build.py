import re
import os
import shutil

BUILD_PATH  = 'ios_build'
LIBPD_PATH  = 'ios_libpd'
PD_FILE     = 'kalimbaTest.pd'
ASSETS_PATH = '../Assets'

if __name__ == '__main__':

    print('Modifying the code...')

    # filepath: [(line number, code),...]
    modifications = {

    'Classes/UnityAppController.mm': [
        (67, '@synthesize audioController = audioController_;\n\n'),
        (89, '''
        self.audioController = [[[PdAudioController alloc] init] autorelease];
        [self.audioController configurePlaybackWithSampleRate:44100 numberChannels:2 inputEnabled:YES mixingEnabled:NO];

        [self.audioController configureTicksPerBuffer:128];

        [PdBase openFile:@"{pd_file}" path:[[NSBundle mainBundle] resourcePath]];
        [self.audioController setActive:YES];
        [self.audioController print];\n'''.format(pd_file=PD_FILE))
    ],

    'Classes/UnityAppController.h': [
        (6, '#import "PdAudioController.h"\n#import "PdBase.h"\n'),
        (27, '\n@property (nonatomic, retain) PdAudioController *audioController;\n')
    ]

    }

    for file, injections in modifications.items():
        filepath = os.path.join(BUILD_PATH, file)
        with open(filepath, 'r') as f:
            contents = f.readlines()

        for index, code in injections:
            contents.insert(index, code)

        with open(filepath, 'w') as f:
            f.write(''.join(contents))

    print('Copying over files...')
    patches_path = os.path.join(BUILD_PATH, 'Patches')
    os.makedirs(patches_path)
    shutil.copy(os.path.join(ASSETS_PATH, 'StreamingAssets/pd', PD_FILE), patches_path)
    shutil.copytree(LIBPD_PATH, os.path.join(BUILD_PATH, 'ios_libpd'))


    print('Copying the prepared pbxproj file...')
    proj_path = os.path.join(BUILD_PATH, 'Unity-iPhone.xcodeproj/project.pbxproj')
    os.remove(proj_path)
    shutil.copy('project.pbxproj', proj_path)

    print('All finished!')
