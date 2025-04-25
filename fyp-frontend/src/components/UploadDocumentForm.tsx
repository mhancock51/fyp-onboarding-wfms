import { Label } from '@radix-ui/react-dropdown-menu'
import React, { useState } from 'react'
import { Input } from './ui/input';
import { Button } from './ui/button';
import { Upload } from 'lucide-react';
import { Spinner } from './ui/spinner';
import Api from '@/api';
import { toast } from 'sonner';

interface Props {
  allowedFileExtensions: string;
  documentName: string;
  accessAccountIds: string[];
  taskInstanceId?: string;
  onSuccessfullUpload?: () => void;
}

export default function UploadDocumentForm(props: Props) {
  const [file, setFile] = useState<File | null>(null);
  const [uploading, setUploading] = useState<boolean>(false);

  function handleFileInputChange(event: React.ChangeEvent<HTMLInputElement>) {
      if (event.target.files && event.target.files.length > 0) {
        setFile(event.target.files[0]);
      }
    }

  async function uploadDocument() {
    setUploading(true);
    if (file === null) return;
    Api.documents.uploadDocument(file, props.documentName, props.accessAccountIds, props.taskInstanceId)
    .then(() => {
      toast.success("Successfully uploaded document");
      if (props.onSuccessfullUpload) {
        props.onSuccessfullUpload();
      }
    })
    .catch((error) => {
      toast.error("Failed to upload document");
      console.log(error);
    })
    .finally(() => {
      setUploading(false);
    });
  }


  return (
    <form className='flex flex-col gap-1 my-4 w-full item-center justify-center text-center'
      onSubmit={(event: any) => {event.preventDefault(); void uploadDocument()}}
    >       
      <Input accept={props.allowedFileExtensions.split(";").join(", ")} type="file" required className='bg-gray-100 px-2 cursor-pointer' onChange={handleFileInputChange}/>
      <Label className='text-sm'>Allowed document types: {props.allowedFileExtensions.split(";").join(", ")}</Label>
      <Button type='submit'>
        <div className='flex flex-row w-full justify-center gap-2 items-center'>
          Upload Document
          <Upload/>
          {
            uploading &&
            <Spinner className="text-primary-foreground"/>
          }
        </div>
      </Button>            
    </form>
  )
}