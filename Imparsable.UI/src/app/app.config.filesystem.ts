import {configure, Dirent} from '@zenfs/core';
import {mkdir, readdir, exists, writeFile} from '@zenfs/core/promises';
import {IndexedDB} from '@zenfs/dom';

const workspaceRoot = '/workspace';
const calculatorWorkspace = `${workspaceRoot}/calculator`;
const calculatorFile = `${calculatorWorkspace}/test.clc`;
const code: string = `const pi = 3.14;
const radius = 4 / 2;
var area = 2 * pi * radius;
print "Area" + ': ' + area;
print 1 + 2;

for (var x = 0; x < 3; x += 1)
    print x + 1;
`;

export async function initializeFileSystem(): Promise<void> {
  const mounts: any = {};
  mounts[`${workspaceRoot}`] = IndexedDB;

  await configure({mounts: mounts});
  await mkdir(calculatorWorkspace, {recursive: true});

  if (!await exists(calculatorFile)) {
    await writeFile(calculatorFile, code);
  }
}

export async function readCalculatorWorkspace(): Promise<Dirent[]> {
  return (await readdir(calculatorWorkspace, {recursive: true, withFileTypes: true})).sort();
}
