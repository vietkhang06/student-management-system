package com.demo.student_management.entity;

import jakarta.persistence.Column;
import jakarta.persistence.Embeddable;
import lombok.*;

import java.io.Serializable;

@Embeddable
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@EqualsAndHashCode
public class ChiTietDiemId implements Serializable {

    @Column(name = "ID_HOCSINH", length = 10, nullable = false)
    private String idHocSinh;

    @Column(name = "ID_MONHOC", length = 20, nullable = false)
    private String idMonHoc;

    @Column(name = "ID_HOCKY", length = 20, nullable = false)
    private String idHocKy;
}