export interface PendingAnnotation {
  wkt: string;
  x:   number;   // context menü konumu (px, tooltip ile aynı desende)
  y:   number;
}

export interface AnnotationResponseDto {
  id:          number;
  noteText:    string;
  wktGeometry: string;
  userId:      number;
  teamId:      number | null;
  createdDate: string;
}