# ChatGPT Plugins �� Semantic Kernel �Ŏg���R���\�[���A�v��

## �t�H���_�\��

- TodoPlugin
  - TODO ���X�g���Ǘ����� ChatGPT Plugins�B�ȉ��̂悤�� TODO ���f�t�H���g�œo�^����Ă���B
	```json
	[
        {
            "text": "�g�C���b�g�y�[�p�[�𔃂��ɍs��"
        },
        {
            "text": "�����𔃂��ɍs��"
        },
        {
            "text": "�����ւ̗��s�̓��������߂�"
        },
        {
            "text": "2023�N7���ɏo��X�^�o�̐V�삪�o����H�ׂ�"
        }
    ]
    ```

- MyConsoleApp
  - TodoPlugin ��ǂݍ���œ����R���\�[���A�v���̃`���b�g�v���O�����B��L�� TODO ���X�g�̃v���O�C����K�v�ɉ����ČĂяo���ē��삷��B
    
    �����
    ```
    User: TODO�̈ꗗ��\�����āB
    �����𓥂܂�����ł̃��[�U�[�̈Ӑ}: TODO���X�g�̈ꗗ��\������B
    Assistant: �ȉ������݂�TODO���X�g�ł��B

    - �g�C���b�g�y�[�p�[�𔃂��ɍs��
    - �����𔃂��ɍs��
    - �����ւ̗��s�̓��������߂�
    - 2023�N7���ɏo��X�^�o�̐V�삪�o����H�ׂ�

    �ȏ�A���m�F���������B

    > �g�p�v���O�C��: Todos.getTodos()
    User: "�[���𔃂��ɍs��"��TODO�ɒǉ�����
    �����𓥂܂�����ł̃��[�U�[�̈Ӑ}: ���[�U�[�́ATODO���X�g�Ɂu�[���𔃂��ɍs���v��ǉ��������Ǝv���Ă��܂��B
    Assistant: �u�[���𔃂��ɍs���v��TODO���X�g�ɒǉ����܂����B

    �ȉ������݂�TODO���X�g�ł��B

    - �g�C���b�g�y�[�p�[�𔃂��ɍs��
    - �����𔃂��ɍs��
    - �����ւ̗��s�̓��������߂�
    - 2023�N7���ɏo��X�^�o�̐V�삪�o����H�ׂ�
    - �[���𔃂��ɍs��

    �ȏ�A���m�F���������B

    > �g�p�v���O�C��: Todos.createTodo()
    User: ����Ȃ��Ƃ����Ȃ����i���������ꗗ�ŏo���āB
    �����𓥂܂�����ł̃��[�U�[�̈Ӑ}: ���݂�TODO���X�g����A����Ȃ���΂����Ȃ����i���������ꗗ�ŕ\�����Ăق����B
    Assistant: �ȉ������݂�TODO���X�g����A����Ȃ���΂����Ȃ����i�������̈ꗗ�ł��B

    - �g�C���b�g�y�[�p�[�𔃂��ɍs��
    - �����𔃂��ɍs��
    - �[���𔃂��ɍs��

    �ȏ�A���m�F���������B

    > �g�p�v���O�C��: Todos.getTodos()
    User:
    ```

## ��������

### �K�v��

- Visual Studio 2022 or .NET 7 SDK
- Azure OpenAI Service �̃��\�[�X���쐬���� Azure �T�u�X�N���v�V����������
- Azure CLI �� Azure OpenAI Service �̂��� Azure �T�u�X�N���v�V�����փT�C���C�����s���Ă���

### �ݒ�

- MyConsoleApp\appsettings.json �������� Azure OpenAI Service �̃��\�[�X�̃G���h�|�C���g�� gpt-35-turbo �̃f�v���C���ɍX�V����
- Azure OpenAI Service �̂��郊�\�[�X�O���[�v�Ɏ����̃A�J�E���g�ɑ΂��� Cognitive Service OpenAI User �̃��[����ǉ�����

### ���s

- TodoPlugin �v���W�F�N�g�����s����
  - dotnet run �ɂ����s���f�o�b�O�����ŊJ�n���ċN�������ςȂ��ɂ���
- MyConsoleApp �v���W�F�N�g�����s����
